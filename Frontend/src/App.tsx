import React, { useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Tabs,
  Tab,
  AppBar,
  Toolbar,
} from '@mui/material';
import TreasureIcon from '@mui/icons-material/Diamond';
import TreasureHuntSolver from './components/TreasureHuntSolver';
import HistoryViewer from './components/HistoryViewer';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

function App() {
  const [tabValue, setTabValue] = useState(0);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  return (
    <div>
      <AppBar position="static">
        <Toolbar>
          <TreasureIcon sx={{ mr: 2 }} />
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Treasure Hunt Solver
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={tabValue} onChange={handleTabChange} aria-label="basic tabs example">
            <Tab label="Solve Problem" />
            <Tab label="History" />
          </Tabs>
        </Box>
        
        <TabPanel value={tabValue} index={0}>
          <TreasureHuntSolver />
        </TabPanel>
        
        <TabPanel value={tabValue} index={1}>
          <HistoryViewer />
        </TabPanel>
      </Container>
    </div>
  );
}

export default App; 