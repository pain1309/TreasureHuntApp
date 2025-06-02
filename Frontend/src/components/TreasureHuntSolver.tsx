import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Grid,
  Alert,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  Chip,
} from '@mui/material';
import { treasureHuntApi } from '../services/api';
import { TreasureHuntRequest, TreasureHuntResponse } from '../types';

const TreasureHuntSolver: React.FC = () => {
  const [n, setN] = useState<number>(3);
  const [m, setM] = useState<number>(3);
  const [p, setP] = useState<number>(3);
  const [matrix, setMatrix] = useState<number[][]>([
    [3, 2, 2],
    [2, 2, 2],
    [2, 2, 1]
  ]);
  const [algorithmType, setAlgorithmType] = useState<string>('optimal');
  const [result, setResult] = useState<TreasureHuntResponse | null>(null);
  const [comparisonResult, setComparisonResult] = useState<any>(null);
  const [testResults, setTestResults] = useState<any>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');
  const [backendStatus, setBackendStatus] = useState<'unknown' | 'connected' | 'disconnected'>('unknown');
  const [apiUrl, setApiUrl] = useState<string>('');

  // Check backend status on component mount
  useEffect(() => {
    const checkBackendStatus = async () => {
      try {
        const isConnected = await treasureHuntApi.checkConnection();
        if (isConnected) {
          setBackendStatus('connected');
          const currentUrl = treasureHuntApi.getCurrentApiUrl();
          if (currentUrl) {
            setApiUrl(currentUrl);
          }
        } else {
          setBackendStatus('disconnected');
        }
      } catch (error) {
        setBackendStatus('disconnected');
      }
    };
    
    checkBackendStatus();
  }, []);

  const handleDimensionChange = (newN: number, newM: number) => {
    setN(newN);
    setM(newM);
    
    // Create new matrix with default values
    const newMatrix: number[][] = [];
    for (let i = 0; i < newN; i++) {
      newMatrix[i] = [];
      for (let j = 0; j < newM; j++) {
        newMatrix[i][j] = matrix[i] && matrix[i][j] ? matrix[i][j] : 1;
      }
    }
    setMatrix(newMatrix);
  };

  const handleMatrixChange = (row: number, col: number, value: number) => {
    const newMatrix = [...matrix];
    newMatrix[row][col] = value;
    setMatrix(newMatrix);
  };

  const validateInput = (): boolean => {
    if (n <= 0 || m <= 0 || p <= 0) {
      setError('N, M, and P must be positive numbers');
      return false;
    }

    if (n > 500 || m > 500) {
      setError('N and M must be <= 500');
      return false;
    }

    if (p > n * m) {
      setError('P cannot be greater than N × M');
      return false;
    }

    // Check if all values in matrix are valid
    for (let i = 0; i < n; i++) {
      for (let j = 0; j < m; j++) {
        if (matrix[i][j] < 1 || matrix[i][j] > p) {
          setError(`Matrix values must be between 1 and ${p}`);
          return false;
        }
      }
    }

    // Check if all chest numbers from 1 to p exist
    const chestNumbers = new Set<number>();
    for (let i = 0; i < n; i++) {
      for (let j = 0; j < m; j++) {
        chestNumbers.add(matrix[i][j]);
      }
    }

    for (let i = 1; i <= p; i++) {
      if (!chestNumbers.has(i)) {
        setError(`Chest number ${i} is missing from the matrix`);
        return false;
      }
    }

    return true;
  };

  const handleSolve = async () => {
    setError('');
    setResult(null);
    setComparisonResult(null);

    if (!validateInput()) {
      return;
    }

    setLoading(true);

    try {
      const request: TreasureHuntRequest = {
        n,
        m,
        p,
        matrix
      };

      let response;
      if (algorithmType === 'optimal') {
        response = await treasureHuntApi.solveTreasureHunt(request);
      } else if (algorithmType === 'greedy') {
        response = await treasureHuntApi.solveTreasureHuntGreedy(request);
      } else {
        response = await treasureHuntApi.compareBothAlgorithms(request);
        setComparisonResult(response);
        return;
      }

      setResult(response);
      
      // Update backend status on successful call
      setBackendStatus('connected');
      const currentUrl = treasureHuntApi.getCurrentApiUrl();
      if (currentUrl) {
        setApiUrl(currentUrl);
      }
    } catch (err: any) {
      console.error('API Error:', err);
      
      // Handle different types of errors
      let errorMessage = 'Failed to solve treasure hunt';
      
      if (err.message?.includes('Unable to connect')) {
        errorMessage = 'Cannot connect to backend server. Please ensure it is running.';
        setBackendStatus('disconnected');
      } else if (err.message?.includes('API endpoint not found')) {
        errorMessage = 'Backend API endpoint not found. Please check server configuration.';
      } else if (err.message?.includes('Server error')) {
        errorMessage = 'Backend server error. Please try again later.';
      } else if (err.response?.data?.message) {
        errorMessage = err.response.data.message;
      } else if (err.message) {
        errorMessage = err.message;
      }
      
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleRunTests = async () => {
    setLoading(true);
    setError('');
    
    try {
      const results = await treasureHuntApi.runTests();
      setTestResults(results);
      
      // Update backend status on successful call
      setBackendStatus('connected');
      const currentUrl = treasureHuntApi.getCurrentApiUrl();
      if (currentUrl) {
        setApiUrl(currentUrl);
      }
    } catch (err: any) {
      console.error('Test Error:', err);
      
      // Handle different types of errors
      let errorMessage = 'Failed to run tests';
      
      if (err.message?.includes('Unable to connect')) {
        errorMessage = 'Cannot connect to backend server. Please ensure it is running.';
        setBackendStatus('disconnected');
      } else if (err.message?.includes('API endpoint not found')) {
        errorMessage = 'Backend API endpoint not found. Please check server configuration.';
      } else if (err.message?.includes('Server error')) {
        errorMessage = 'Backend server error. Please try again later.';
      } else if (err.message) {
        errorMessage = err.message;
      }
      
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleRetryConnection = async () => {
    setBackendStatus('unknown');
    setError('');
    
    try {
      const isConnected = await treasureHuntApi.checkConnection();
      if (isConnected) {
        setBackendStatus('connected');
        const currentUrl = treasureHuntApi.getCurrentApiUrl();
        if (currentUrl) {
          setApiUrl(currentUrl);
        }
      } else {
        setBackendStatus('disconnected');
        setError('Could not establish connection to backend server');
      }
    } catch (error) {
      setBackendStatus('disconnected');
      setError('Error checking backend connection');
    }
  };

  const loadExample = (exampleNumber: number) => {
    switch (exampleNumber) {
      case 1:
        setN(3);
        setM(3);
        setP(3);
        setMatrix([
          [3, 2, 2],
          [2, 2, 2],
          [2, 2, 1]
        ]);
        break;
      case 2:
        setN(3);
        setM(4);
        setP(3);
        setMatrix([
          [2, 1, 1, 1],
          [1, 1, 1, 1],
          [2, 1, 1, 3]
        ]);
        break;
      case 3:
        setN(3);
        setM(4);
        setP(12);
        setMatrix([
          [1, 2, 3, 4],
          [8, 7, 6, 5],
          [9, 10, 11, 12]
        ]);
        break;
    }
    setResult(null);
    setComparisonResult(null);
    setTestResults(null);
    setError('');
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Treasure Hunt Solver
      </Typography>
      
      {/* Backend Status Indicator */}
      <Box mb={2}>
        <Alert 
          severity={backendStatus === 'connected' ? 'success' : backendStatus === 'disconnected' ? 'error' : 'info'}
          action={
            backendStatus === 'disconnected' && (
              <Button color="inherit" size="small" onClick={handleRetryConnection}>
                Retry
              </Button>
            )
          }
        >
          Backend Status: {
            backendStatus === 'connected' ? `✅ Connected ${apiUrl ? `(${apiUrl})` : ''}` :
            backendStatus === 'disconnected' ? '❌ Disconnected - Please start the backend server' :
            '⏳ Checking connection...'
          }
        </Alert>
      </Box>
      
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Input Parameters
              </Typography>
              
              <Grid container spacing={2}>
                <Grid item xs={4}>
                  <TextField
                    label="N (rows)"
                    type="number"
                    value={n}
                    onChange={(e) => handleDimensionChange(parseInt(e.target.value) || 1, m)}
                    inputProps={{ min: 1, max: 500 }}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={4}>
                  <TextField
                    label="M (columns)"
                    type="number"
                    value={m}
                    onChange={(e) => handleDimensionChange(n, parseInt(e.target.value) || 1)}
                    inputProps={{ min: 1, max: 500 }}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={4}>
                  <TextField
                    label="P (max chest)"
                    type="number"
                    value={p}
                    onChange={(e) => setP(parseInt(e.target.value) || 1)}
                    inputProps={{ min: 1 }}
                    fullWidth
                  />
                </Grid>
              </Grid>

              <Box mt={2}>
                <Typography variant="h6" gutterBottom>
                  Matrix Input
                </Typography>
                <TableContainer component={Paper}>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell></TableCell>
                        {Array.from({ length: m }, (_, j) => (
                          <TableCell key={j} align="center">
                            {j + 1}
                          </TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {Array.from({ length: n }, (_, i) => (
                        <TableRow key={i}>
                          <TableCell component="th" scope="row">
                            {i + 1}
                          </TableCell>
                          {Array.from({ length: m }, (_, j) => (
                            <TableCell key={j}>
                              <TextField
                                type="number"
                                value={matrix[i]?.[j] || 1}
                                onChange={(e) => 
                                  handleMatrixChange(i, j, parseInt(e.target.value) || 1)
                                }
                                inputProps={{ min: 1, max: p }}
                                size="small"
                                sx={{ width: 60 }}
                              />
                            </TableCell>
                          ))}
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Box>

              <Box mt={2}>
                <FormControl component="fieldset">
                  <FormLabel component="legend">Algorithm</FormLabel>
                  <RadioGroup
                    row
                    value={algorithmType}
                    onChange={(e) => setAlgorithmType(e.target.value)}
                  >
                    <FormControlLabel value="optimal" control={<Radio />} label="Optimal (Recommended)" />
                    <FormControlLabel value="greedy" control={<Radio />} label="Greedy" />
                    <FormControlLabel value="compare" control={<Radio />} label="Compare Both" />
                  </RadioGroup>
                </FormControl>
              </Box>

              <Box mt={2}>
                <Typography variant="subtitle2" gutterBottom>
                  Load Examples:
                </Typography>
                <Button onClick={() => loadExample(1)} size="small" sx={{ mr: 1 }}>
                  Example 1
                </Button>
                <Button onClick={() => loadExample(2)} size="small" sx={{ mr: 1 }}>
                  Example 2
                </Button>
                <Button onClick={() => loadExample(3)} size="small" sx={{ mr: 1 }}>
                  Example 3
                </Button>
                <Button onClick={handleRunTests} size="small" color="secondary" sx={{ ml: 2 }}>
                  Run All Tests
                </Button>
              </Box>

              <Box mt={3}>
                <Button
                  variant="contained"
                  onClick={handleSolve}
                  disabled={loading}
                  fullWidth
                  size="large"
                >
                  {loading ? <CircularProgress size={24} /> : 'Solve Treasure Hunt'}
                </Button>
              </Box>

              {error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                  {error}
                </Alert>
              )}
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          {/* Single Algorithm Result */}
          {result && (
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Solution ({algorithmType === 'optimal' ? 'Optimal' : 'Greedy'})
                </Typography>
                
                {result.success ? (
                  <Box>
                    <Alert severity="success" sx={{ mb: 2 }}>
                      Minimum fuel required: <strong>{result.minimumFuel.toFixed(5)}</strong>
                      {algorithmType === 'optimal' && (
                        <Chip label="OPTIMAL" color="success" size="small" sx={{ ml: 1 }} />
                      )}
                    </Alert>
                    
                    <Typography variant="subtitle1" gutterBottom>
                      Path:
                    </Typography>
                    <TableContainer component={Paper}>
                      <Table size="small">
                        <TableHead>
                          <TableRow>
                            <TableCell>Step</TableCell>
                            <TableCell>Position</TableCell>
                            <TableCell>Chest</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {result.path.map((position, index) => (
                            <TableRow key={index}>
                              <TableCell>{index}</TableCell>
                              <TableCell>({position.row}, {position.col})</TableCell>
                              <TableCell>{position.chestNumber}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </Box>
                ) : (
                  <Alert severity="error">
                    {result.errorMessage || 'Failed to solve the problem'}
                  </Alert>
                )}
              </CardContent>
            </Card>
          )}

          {/* Comparison Result */}
          {comparisonResult && (
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Algorithm Comparison
                </Typography>
                
                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <Typography variant="subtitle2" color="text.secondary">Greedy Algorithm</Typography>
                    <Typography variant="h6">{comparisonResult.greedy.minimumFuel.toFixed(5)}</Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="subtitle2" color="text.secondary">Optimal Algorithm</Typography>
                    <Typography variant="h6" color="success.main">
                      {comparisonResult.optimal.minimumFuel.toFixed(5)}
                    </Typography>
                  </Grid>
                </Grid>
                
                <Alert severity="info" sx={{ mt: 2 }}>
                  Improvement: <strong>{comparisonResult.improvement}</strong> better with optimal algorithm
                </Alert>

                <Box mt={2}>
                  <Typography variant="subtitle2" gutterBottom>Optimal Path:</Typography>
                  <TableContainer component={Paper} sx={{ maxHeight: 200 }}>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Step</TableCell>
                          <TableCell>Position</TableCell>
                          <TableCell>Chest</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {comparisonResult.optimal.path?.map((position: any, index: number) => (
                          <TableRow key={index}>
                            <TableCell>{index}</TableCell>
                            <TableCell>({position.row}, {position.col})</TableCell>
                            <TableCell>{position.chestNumber}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </Box>
              </CardContent>
            </Card>
          )}

          {/* Test Results */}
          {testResults && (
            <Card sx={{ mt: 2 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Test Results
                </Typography>
                {testResults.map((test: any, index: number) => (
                  <Box key={index} sx={{ mb: 2 }}>
                    <Typography variant="subtitle2">Test Case {test.testCase}</Typography>
                    <Typography variant="body2">
                      Expected: {test.expected.toFixed(5)} | 
                      Greedy: {test.greedy.toFixed(5)} | 
                      Optimal: <strong style={{color: test.optimal <= test.expected + 0.001 ? 'green' : 'orange'}}>
                        {test.optimal.toFixed(5)}
                      </strong>
                    </Typography>
                  </Box>
                ))}
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>
    </Box>
  );
};

export default TreasureHuntSolver; 