using UnityEngine;

namespace Assets.Metater.MetaVoiceChat.Input
{
    public abstract class VcInputFilter : MonoBehaviour
    {
        [Tooltip("The next audio input filter in the pipeline. This can be null.")]
        public VcInputFilter nextInputFilter;

        /// <summary>
        /// Usage: Setting the samples array to null will stop the pipeline and signal that the samples should not be sent. The incoming samples array may be null.
        /// </summary>
        protected abstract void Filter(int index, ref float[] samples);

        public void FilterRecursively(int index, ref float[] samples)
        {
            VcInputFilter targetInputFilter = this;
            while (targetInputFilter != null && samples != null)
            {
                if (targetInputFilter.isActiveAndEnabled)
                {
                    targetInputFilter.Filter(index, ref samples);
                }

                targetInputFilter = targetInputFilter.nextInputFilter;
            }
        }
    }
}