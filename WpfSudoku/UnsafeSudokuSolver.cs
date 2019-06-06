using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSudoku.UnsafeSudokuSolver {
    public class UnsafeSudokuModel {
        private int[] Grid;
        public UnsafeSudokuModel(int[] grid) {
            this.Grid = new int[81];
            Array.Copy(grid, this.Grid, 81);
        }

        private unsafe int RowStart(int index) => (9 * (index / 9));
        private unsafe int ColumnStart(int index) => (index % 9);
        private unsafe int BlockRowStart(int index) => 3 * ((index / 9) / 3);
        private unsafe int BlockColumnStart(int index) => 3 * ((index % 9) / 3);
        private unsafe bool CheckRow(int* grid, int index, int value) {
            int* rowStart = grid + RowStart(index);
            for (int i = 0; i < 9; i++) {
                if (value == *(rowStart + i)) return false;
            }
            return true;
        }
        private unsafe bool CheckColumn(int* grid, int index, int value) {
            int* columnStart = grid + ColumnStart(index);
            for (int i = 0; i < 9; i++) {
                if (value == *(columnStart + (9 * i))) return false;
            }
            return true;
        }
        private unsafe bool CheckBlock(int* grid, int index, int value) {
            int blockRowStart = BlockRowStart(index);
            int blockColumnStart = BlockColumnStart(index);
            int* blockStart = grid + (9 * blockRowStart) + blockColumnStart;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (value == *(blockStart + (9 * i) + j)) return false;
                }
            }
            return true;
        }
        private unsafe bool IsValid(int* grid, int index, int value) {
            return CheckRow(grid, index, value) && CheckColumn(grid, index, value) && CheckBlock(grid, index, value);
        }
        private unsafe bool Backtrack(int* grid, int index, int value) {
            if (index == 81) return true;
            if (*(grid + index) != 0) return Backtrack(grid, index + 1, 1);
            if (IsValid(grid, index, value)) {
                *(grid + index) = value;
                if (Backtrack(grid, index + 1, 1)) {
                    return true;
                } else {
                    *(grid + index) = 0;
                    return NextValue(grid, index, value);
                }
            } else {
                return NextValue(grid, index, value);
            }

        }

        private unsafe bool NextValue(int* grid, int index, int value) {
            if (value < 9) return Backtrack(grid, index, value + 1);
            return false;
        }
        private unsafe int[] USolve() {
            fixed (int* _grid = this.Grid) {
                Backtrack(_grid, 0, 1);
            }
            return this.Grid;
        }
        public int[] Solve() {
            return USolve();
        }
    }
}
