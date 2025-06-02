import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Alert,
  CircularProgress,
  IconButton,
} from '@mui/material';
import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import DeleteIcon from '@mui/icons-material/Delete';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import { treasureHuntApi } from '../services/api';
import { TreasureMatrix, TreasureHuntResponse } from '../types';

const HistoryViewer: React.FC = () => {
  const [history, setHistory] = useState<TreasureMatrix[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');
  const [replayResult, setReplayResult] = useState<TreasureHuntResponse | null>(null);

  const loadHistory = async () => {
    setLoading(true);
    setError('');
    
    try {
      const data = await treasureHuntApi.getHistory();
      setHistory(data);
    } catch (err: any) {
      setError('Failed to load history');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await treasureHuntApi.deleteHistoryItem(id);
      setHistory(history.filter(item => item.id !== id));
    } catch (err: any) {
      setError('Failed to delete item');
    }
  };

  const handleReplay = async (id: number) => {
    try {
      const result = await treasureHuntApi.replayFromHistory(id);
      setReplayResult(result);
    } catch (err: any) {
      setError('Failed to replay solution');
    }
  };

  useEffect(() => {
    loadHistory();
  }, []);

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'n', headerName: 'N', width: 70 },
    { field: 'm', headerName: 'M', width: 70 },
    { field: 'p', headerName: 'P', width: 70 },
    { 
      field: 'result', 
      headerName: 'Result', 
      width: 120,
      valueFormatter: (params) => params.value.toFixed(5)
    },
    { 
      field: 'createdAt', 
      headerName: 'Created At', 
      width: 180,
      valueFormatter: (params) => new Date(params.value).toLocaleString()
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params) => [
        <GridActionsCellItem
          icon={<PlayArrowIcon />}
          label="Replay"
          onClick={() => handleReplay(params.row.id)}
        />,
        <GridActionsCellItem
          icon={<DeleteIcon />}
          label="Delete"
          onClick={() => handleDelete(params.row.id)}
        />,
      ],
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Solution History
      </Typography>
      
      <Card>
        <CardContent>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography variant="h6">
              Saved Solutions
            </Typography>
            <Button variant="outlined" onClick={loadHistory} disabled={loading}>
              {loading ? <CircularProgress size={20} /> : 'Refresh'}
            </Button>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box height={400}>
            <DataGrid
              rows={history}
              columns={columns}
              initialState={{
                pagination: {
                  paginationModel: { page: 0, pageSize: 10 },
                },
              }}
              pageSizeOptions={[10]}
              disableRowSelectionOnClick
              loading={loading}
            />
          </Box>
        </CardContent>
      </Card>

      {replayResult && (
        <Card sx={{ mt: 3 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Replay Result
            </Typography>
            
            {replayResult.success ? (
              <Alert severity="success">
                Minimum fuel required: <strong>{replayResult.minimumFuel.toFixed(5)}</strong>
              </Alert>
            ) : (
              <Alert severity="error">
                {replayResult.errorMessage || 'Failed to replay solution'}
              </Alert>
            )}
          </CardContent>
        </Card>
      )}
    </Box>
  );
};

export default HistoryViewer; 