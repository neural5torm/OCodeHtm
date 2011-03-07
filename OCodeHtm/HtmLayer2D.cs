using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;
using System.Threading.Tasks;

namespace CnrsUniProv.OCodeHtm
{
    public /*abstract*/ class HtmLayer2D : HtmLayer<SparseMatrix, SparseVector>
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
        
        public ILearningInferring<SparseMatrix, Vector>[,] NodeArray { get; protected set; }



        public HtmLayer2D(uint width, uint height, double overlap, bool clone, uint maxOutputSize)
            : base(overlap, clone, maxOutputSize)
        {
            Height = (int)height;
            Width = (int)width;

            DoInitializeNodes();
            State = LayerState.Learning;
            
        }



        protected /*abstract*/ void DoInitializeNodes() { }

        public override void Learn(SparseMatrix input)
        {
            // Rule check
            if (State == LayerState.Trained)
                // TODOlater allow learning after training when using FixedMaxSize nodes
                throw new HtmRuleException("Cannot learn after any other mode than learning", this);

            // Learn in parallel, if possible
            var actions = new List<Action>();

            if (IsTrainedNodeCloned)
            {
                actions.Add(() =>
                {
                    ClonedNode.Learn(GetSubMatrixForNodeAt(ClonedNodeRow, ClonedNodeCol, input));
                });
            }
            else
            {
                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < Width; col++)
                    {
                        actions.Add(() =>
                        {
                            NodeArray[row, col].Learn(GetSubMatrixForNodeAt(row, col, input));
                        });
                    }
                }
            }

            Parallel.Invoke(actions.ToArray());
        }

        public override SparseMatrix Infer(SparseMatrix input)
        {
            // In clone mode, clone the trained node when inferring for the first time (right after training)
            if (State == LayerState.Learning && IsTrainedNodeCloned)
                DoCloneTrainedNode();

            State = LayerState.Trained;

            // Create inferrence output in parallel
            var outputs = new Matrix[Height, Width];
            var actions = new List<Action>();
            
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    actions.Add(() =>
                    {
                        outputs[row, col] = (Matrix)NodeArray[row, col].Infer(GetSubMatrixForNodeAt(row, col, input)).ToRowMatrix();

                        
                        
                    });
                }
            }
            Parallel.Invoke(actions.ToArray());

            // Concatenate outputs into one big output matrix
            var output = new SparseMatrix(Height, Width * MaxNodeOutputSize);
            for (int row = 0; row < Height; ++row)
            {
                for(int col = 0; col < Width; ++col)
                {
                     output.SetSubMatrix(row, 1, col * MaxNodeOutputSize, outputs[row, col].ColumnCount, outputs[row, col]);
                }
            }
            return output;
        }

        protected /*abstract*/ void DoCloneTrainedNode()
        {
            throw new NotImplementedException();
        }


        public SparseMatrix GetSubMatrixForNodeAt(int nodeRow, int nodeColumn, SparseMatrix input)
        {
            var subInputHeight = (double)input.RowCount / Height + InputOverlapRatio * (input.RowCount - (double)input.RowCount / Height);
            var deltaHeight = (input.RowCount - subInputHeight) / (Height - 1);
            var subInputWidth = (double)input.ColumnCount / Width + InputOverlapRatio * (input.ColumnCount - (double)input.ColumnCount / Width);
            var deltaWidth = (input.ColumnCount - subInputWidth) / (Width - 1);

            var matrixRowOrigin = Convert.ToInt32(nodeRow * deltaHeight);
            var matrixColOrigin = Convert.ToInt32(nodeColumn * deltaWidth);

            return (SparseMatrix)input.SubMatrix(matrixRowOrigin, Convert.ToInt32(nodeRow * deltaHeight + subInputHeight) - matrixRowOrigin,
                                                matrixColOrigin, Convert.ToInt32(nodeColumn * deltaWidth + subInputWidth) - matrixColOrigin);
        }

       
    }
}
