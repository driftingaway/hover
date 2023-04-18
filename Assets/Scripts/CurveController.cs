using UnityEngine;

namespace AIEngineTest
{
    public class CurveController : MonoBehaviour
    {
        private static CurveController s_Instance = null;

        private const float k_StraightDistanceClamp = 100f;
        private const float k_CurvatureClamp = 10f;

        [SerializeField] [Range(0f, k_StraightDistanceClamp)] private float m_StraightDistance = 10f;
        [SerializeField] [Range(-k_CurvatureClamp, k_CurvatureClamp)] private float m_VerticalCurvature = 0f;
        [SerializeField] [Range(-k_CurvatureClamp, k_CurvatureClamp)] private float m_HorizontalCurvature = 0f;

        private static readonly int s_StraightRenderDistanceID = Shader.PropertyToID("_CURVER_STRAIGHT_RENDER_DISTANCE");
        private static readonly int s_HorizontalCurvatureID = Shader.PropertyToID("_CURVER_HORIZONTAL_CURVATURE");
        private static readonly int s_VerticalCurvatureID = Shader.PropertyToID("_CURVER_VERTICAL_CURVATURE");

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Debug.Log("Duplicate curve controller detected.", this);
                return;
            }

            UpdateValues();
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        private void OnValidate()
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            straightDistance = m_StraightDistance;
            horizontalCurvature = m_HorizontalCurvature;
            verticalCurvature = m_VerticalCurvature;
        }

        public float straightDistance
        {
            get => m_StraightDistance;
            set
            {
                value = Mathf.Clamp(value, 0f, k_StraightDistanceClamp);
                m_StraightDistance = value;
                Shader.SetGlobalFloat(s_StraightRenderDistanceID, value);
            }
        }

        public float horizontalCurvature
        {
            get => m_HorizontalCurvature;
            set
            {
                value = Mathf.Clamp(value, -k_CurvatureClamp, k_CurvatureClamp);
                m_HorizontalCurvature = value;
                Shader.SetGlobalFloat(s_HorizontalCurvatureID, value);
            }
        }

        public float verticalCurvature
        {
            get => m_VerticalCurvature;
            set
            {
                value = Mathf.Clamp(value, -k_CurvatureClamp, k_CurvatureClamp);
                m_VerticalCurvature = value;
                Shader.SetGlobalFloat(s_VerticalCurvatureID, value);
            }
        }
    }
}
