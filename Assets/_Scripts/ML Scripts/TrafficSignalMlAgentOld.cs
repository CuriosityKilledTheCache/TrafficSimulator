using Simulator.ScriptableObject;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace Simulator.ML {
    public class TrafficSignalMlAgent : Agent {

        const int ACTION_BUFFER_SIZE = 1024;
        private readonly float[] ActionReceiveBuffer = new float[ACTION_BUFFER_SIZE];
        private int actionIndex = 0;
        private List<float> observations = new(121);

        public GameSettingsSO gameSettings;

        public float ConsumeAction(float reward, float[] obseve) {
            AddReward(reward);
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
            observations = obseve.ToList();

            float sum = 0;
            foreach (var action in ActionReceiveBuffer) {
                sum += action;
            }
            //print(sum);

            return sum / ACTION_BUFFER_SIZE;
        }

        private void AddToActionBuffer(float action) {
            actionIndex = (actionIndex + 1) % ACTION_BUFFER_SIZE;
            ActionReceiveBuffer[actionIndex] = action;
        }

        //protected override void Awake() {

        //}



        protected override void Awake() {
            base.Awake();
            //if (!gameSettings.usML)
            //    enabled = false;

            //decisionCoroutine = StartCoroutine(DecisionCoroutine());
            //StartCoroutine(Temp());

            Academy.Instance.AutomaticSteppingEnabled = false;

            //InvokeRepeating(nameof(Test), 0f, 5f);
        }

        //public override void OnEpisodeBegin() {
        //base.OnEpisodeBegin();
        //Reset();
        //}
        void Test() {
            print("Decision requested");
            Academy.Instance.EnvironmentStep();
            print("Decision complete");

        }

        //public void Reset() {
        //    //requested = true;

        //}

        public override void Initialize() {
            base.Initialize();
            //Academy.Instance.AutomaticSteppingEnabled = false;
            //Academy.Instance.EnvironmentStep();
        }

        public override void CollectObservations(VectorSensor sensor) {
            //base.CollectObservations(sensor);
            sensor.AddObservation(observations);
        }

        public override void OnActionReceived(ActionBuffers actions) {
            //base.OnActionReceived(actions);
            //float time = actions.ContinuousActions[0];
            AddToActionBuffer(actions.ContinuousActions[0]);
            print($"action received: {actions.ContinuousActions[0]}");



        }

        //public override void Heuristic(in ActionBuffers actionsOut) {
        //    //base.Heuristic(actionsOut);
        //    ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //    continuousActions[0] = 0f;

        //}

        //public void Test() {
        //    AddReward(-1f);
        //    Debug.Log(GetCumulativeReward());
        //    EndEpisode();
        //}

    }
}