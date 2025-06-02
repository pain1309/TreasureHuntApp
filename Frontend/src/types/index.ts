export interface TreasureHuntRequest {
  n: number;
  m: number;
  p: number;
  matrix: number[][];
}

export interface Position {
  row: number;
  col: number;
  chestNumber: number;
}

export interface TreasureHuntResponse {
  minimumFuel: number;
  path: Position[];
  success: boolean;
  errorMessage?: string;
}

export interface TreasureMatrix {
  id: number;
  n: number;
  m: number;
  p: number;
  matrixData: string;
  result: number;
  createdAt: string;
  solutionPath?: string;
} 