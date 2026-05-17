using System.Collections;
using System.Collections.Generic;

namespace IGP
{
    public class Executable
    {
        readonly List<Node> evaluationOrder;
        readonly List<SequentialNode> sequentialNodes;
        readonly List<PassiveNode> passiveNodes;

        public Executable(List<Node> evaluationOrder, List<SequentialNode> sequentialNodes, List<PassiveNode> passiveNodes)
        {
            this.evaluationOrder = evaluationOrder;
            this.sequentialNodes = sequentialNodes;
            this.passiveNodes = passiveNodes;
        }

        public void Inject(GameAPI gameAPI)
        {
            foreach (var seq in sequentialNodes)
            {
                seq.Inject(gameAPI);
            }
            foreach (var node in evaluationOrder)
            {
                node.Inject(gameAPI);
            }
            foreach (var pasv in passiveNodes)
            {
                pasv.Inject(gameAPI);
            }
        }

        public void Init()
        {
            foreach (var seq in sequentialNodes)
            {
                seq.Init();
            }
            foreach (var node in evaluationOrder)
            {
                node.Init();
            }
            foreach (var pasv in passiveNodes)
            {
                pasv.Init();
            }
        }

        public void Tick(float deltaTime)
        {
            // FF状態確定
            foreach (var seq in sequentialNodes)
            {
                seq.Commit();
            }

            // 組み合わせ回路評価
            foreach (var node in evaluationOrder)
            {
                node.Evaluate(deltaTime);
            }

            // 出力
            foreach (var pasv in passiveNodes)
            {
                pasv.Evaluate(deltaTime);
            }

            // FFの次状態計算
            foreach (var seq in sequentialNodes)
            {
                seq.ComputeNext(deltaTime);
            }
        }
    }
}
