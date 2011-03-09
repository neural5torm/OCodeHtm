using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;
using System.Threading.Tasks;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class HtmLayer2D : HtmLayer<SparseMatrix, Vector>
    {
        public int Height { get; private set; } 
        public int Width { get; private set; }

        public int ClonedNodeRow
        {
            get { return Height / 2; }
        }
        public int ClonedNodeCol
        {
            get { return Width / 2; }
        }
        
        public INode<SparseMatrix, Vector>[,] NodeArray { get; protected set; }



        public HtmLayer2D(uint height, uint width, double overlap, bool clone, uint maxOutputSize)
            : base(overlap, clone, maxOutputSize)
        {
            Height = (int)height;
            Width = (int)width;

            CreateNodes(); // use a void Factory Method to instantiate nodes internally
            State = LayerState.Learning;
            
        }



        protected abstract void CreateNodes();


        public override void Learn(SparseMatrix input)
        {
            // Rule check
            if (State == LayerState.Trained)
                // TODOlater allow learning after training when using FixedMaxSize nodes
                throw new HtmRuleException("Cannot learn after any other mode than learning", this);

            // Learn in parallel, if possible
            if (IsTrainedNodeCloned)
            {
                ClonedNode.Learn(GetSubMatrixForNodeAt(ClonedNodeRow, ClonedNodeCol, input));
            }
            else
            {
                Parallel.For(0, Height, (row) =>
                {
                    Parallel.For(0, Width, (col) =>
                    {
                        NodeArray[row, col].Learn(GetSubMatrixForNodeAt(row, col, input));
                    });
                });
            }
        }

        public override SparseMatrix Infer(SparseMatrix input)
        {
            // In clone mode, clone the trained node when inferring for the first time (right after training)
            if (State == LayerState.Learning && IsTrainedNodeCloned)
                CloneTrainedNode();

            State = LayerState.Trained;

            // Create inferrence outputs in parallel
            var outputs = new Matrix[Height, Width];

            Parallel.For(0, Height, (row) =>
            {
                Parallel.For(0, Width, (col) =>
                {
                    outputs[row, col] = (Matrix)NodeArray[row, col].Infer(GetSubMatrixForNodeAt(row, col, input)).ToRowMatrix();
                });
            });

            // Concatenate outputs into one big matrix (in parallel)
            var output = new SparseMatrix(Height, Width * MaxNodeOutputSize);

            Parallel.For(0, Height, (row) =>
            {
                Parallel.For(0, Width, (col) =>
                {
                    output.SetSubMatrix(row, 1, col * MaxNodeOutputSize, outputs[row, col].ColumnCount, outputs[row, col]);
                });
            });
            return output;
        }


        protected abstract void CloneTrainedNode();

        public SparseMatrix GetSubMatrixForNodeAt(int nodeRow, int nodeColumn, SparseMatrix input)
        {
            var subInputHeight = (double)input.RowCount / Height + InputOverlapRatio * (input.RowCount - (double)input.RowCount / Height);
            var deltaHeight = (input.RowCount - subInputHeight) / (Height - 1);
            var subInputWidth = (double)input.ColumnCount / Width + InputOverlapRatio * (input.ColumnCount - (double)input.ColumnCount / Width);
            var deltaWidth = (input.ColumnCount - subInputWidth) / (Width - 1);

            deltaHeight = deltaHeight.Equals(double.NaN) ? 0 : deltaHeight;
            deltaWidth = deltaWidth.Equals(double.NaN) ? 0 : deltaWidth;

            var matrixRowOrigin = Convert.ToInt32(nodeRow * deltaHeight);
            var matrixColOrigin = Convert.ToInt32(nodeColumn * deltaWidth);

            return (SparseMatrix)input.SubMatrix(matrixRowOrigin, Convert.ToInt32(nodeRow * deltaHeight + subInputHeight) - matrixRowOrigin,
                                                matrixColOrigin, Convert.ToInt32(nodeColumn * deltaWidth + subInputWidth) - matrixColOrigin);
        }

       
    }
}
