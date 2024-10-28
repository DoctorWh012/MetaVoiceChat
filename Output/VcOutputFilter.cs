using UnityEngine;

namespace Assets.Metater.MetaVoiceChat.Output
{
    public abstract class VcOutputFilter : MonoBehaviour
    {
        [Tooltip("The next audio output filter in the pipeline. This can be null.")]
        public VcOutputFilter nextOutputFilter;

        /// <summary>
        /// Usage: Directly modify the samples array to achieve the desired filter. The incoming samples array may be null.
        /// </summary>
        protected abstract void Filter(int index, float[] samples, float targetLatency);

        public void FilterRecursively(int index, float[] samples, float targetLatency)
        {
            VcOutputFilter targetOutputFilter = this;
            while (targetOutputFilter != null && samples != null)
            {
                if (targetOutputFilter.isActiveAndEnabled)
                {
                    targetOutputFilter.Filter(index, samples, targetLatency);
                }

                targetOutputFilter = targetOutputFilter.nextOutputFilter;
            }
        }
    }
}