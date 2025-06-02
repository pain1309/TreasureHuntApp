import axios from 'axios';
import { TreasureHuntRequest, TreasureHuntResponse, TreasureMatrix } from '../types';

// Try multiple possible backend URLs
const API_URLS = [
  'https://localhost:7000/api',
  'http://localhost:5000/api',
  'https://localhost:5000/api',  
  'http://localhost:7000/api'
];

const createApiInstance = () => {
  const api = axios.create({
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Add response interceptor for better error handling
  api.interceptors.response.use(
    (response) => response,
    (error) => {
      console.error('API Error:', error);
      if (error.code === 'ECONNREFUSED' || error.code === 'ERR_NETWORK') {
        throw new Error('Unable to connect to backend server. Please ensure the backend is running.');
      }
      if (error.response?.status === 404) {
        throw new Error('API endpoint not found. Please check backend configuration.');
      }
      if (error.response?.status >= 500) {
        throw new Error('Server error. Please try again later.');
      }
      throw error;
    }
  );

  return api;
};

const api = createApiInstance();

// Auto-detect working API URL
let workingApiUrl: string | null = null;

const getWorkingApiUrl = async (): Promise<string> => {
  if (workingApiUrl) {
    return workingApiUrl;
  }

  for (const url of API_URLS) {
    try {
      api.defaults.baseURL = url;
      await api.get('/treasurehunt/history', { timeout: 3000 });
      workingApiUrl = url;
      console.log(`Connected to backend at: ${url}`);
      return url;
    } catch (error) {
      console.log(`Failed to connect to: ${url}`);
      continue;
    }
  }

  throw new Error('Could not connect to any backend server. Please ensure the backend is running on one of the expected ports.');
};

const makeApiCall = async <T>(apiCall: () => Promise<T>): Promise<T> => {
  try {
    if (!workingApiUrl) {
      await getWorkingApiUrl();
    }
    return await apiCall();
  } catch (error: any) {
    // If connection fails, try to re-detect working URL
    if (error.code === 'ECONNREFUSED' || error.code === 'ERR_NETWORK') {
      workingApiUrl = null;
      await getWorkingApiUrl();
      return await apiCall();
    }
    throw error;
  }
};

export const treasureHuntApi = {
  // Use optimal algorithm by default
  solveTreasureHunt: async (request: TreasureHuntRequest): Promise<TreasureHuntResponse> => {
    return makeApiCall(async () => {
      const response = await api.post<TreasureHuntResponse>('/treasurehunt/solve', request);
      return response.data;
    });
  },

  // Use greedy algorithm specifically
  solveTreasureHuntGreedy: async (request: TreasureHuntRequest): Promise<TreasureHuntResponse> => {
    return makeApiCall(async () => {
      const response = await api.post<TreasureHuntResponse>('/treasurehunt/solve/greedy', request);
      return response.data;
    });
  },

  // Compare both algorithms
  compareBothAlgorithms: async (request: TreasureHuntRequest): Promise<any> => {
    return makeApiCall(async () => {
      const response = await api.post('/treasurehunt/compare', request);
      return response.data;
    });
  },

  // Run built-in tests
  runTests: async (): Promise<any> => {
    return makeApiCall(async () => {
      const response = await api.get('/treasurehunt/test');
      return response.data;
    });
  },

  // Check backend connection
  checkConnection: async (): Promise<boolean> => {
    try {
      await getWorkingApiUrl();
      return true;
    } catch (error) {
      return false;
    }
  },

  // Get current API URL
  getCurrentApiUrl: (): string | null => {
    return workingApiUrl;
  },

  getHistory: async (): Promise<TreasureMatrix[]> => {
    return makeApiCall(async () => {
      const response = await api.get<TreasureMatrix[]>('/treasurehunt/history');
      return response.data;
    });
  },

  getHistoryItem: async (id: number): Promise<TreasureMatrix> => {
    return makeApiCall(async () => {
      const response = await api.get<TreasureMatrix>(`/treasurehunt/history/${id}`);
      return response.data;
    });
  },

  deleteHistoryItem: async (id: number): Promise<void> => {
    return makeApiCall(async () => {
      await api.delete(`/treasurehunt/history/${id}`);
    });
  },

  replayFromHistory: async (id: number): Promise<TreasureHuntResponse> => {
    return makeApiCall(async () => {
      const response = await api.post<TreasureHuntResponse>(`/treasurehunt/replay/${id}`);
      return response.data;
    });
  },
}; 