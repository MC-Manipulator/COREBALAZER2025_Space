using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test
{
    public class CameraFollow : MonoBehaviour
    {
        //DeepSeek写的
        [Header("跟随设置")]
        [Tooltip("要跟随的目标物体")]
        public Transform target;          // 跟随目标

        [Tooltip("摄像机偏移量（相对于目标位置）")]
        public Vector3 offset = new Vector3(0f, 2f, -5f); // 默认偏移量

        [Tooltip("跟随平滑系数（值越小越平滑）")]
        [Range(0.01f, 1f)]
        public float smoothSpeed = 0.125f;

        [Header("旋转设置")]
        [Tooltip("是否启用看向目标")]
        public bool lookAtTarget = true;

        [Tooltip("旋转平滑系数")]
        [Range(0.1f, 5f)]
        public float rotationSmooth = 1f;

        private Vector3 velocity = Vector3.zero;

        void Start()
        {
            // 初始位置设置
            if (target != null)
            {
                transform.position = target.position + offset;
            }
            else
            {
                Debug.LogWarning("未指定跟随目标！请设置 Target。");
            }
        }

        void LateUpdate()
        {
            if (target == null) return;

            // 计算目标位置（带偏移）
            Vector3 targetPosition = target.position +
                                    target.right * offset.x +
                                    target.up * offset.y +
                                    target.forward * offset.z;

            // 平滑移动
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothSpeed);

            // 处理旋转
            if (lookAtTarget)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmooth * Time.deltaTime);
            }
        }

        // 在编辑器中实时预览偏移量（可选）
        void OnDrawGizmosSelected()
        {
            if (target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(target.position, target.position + offset);
                Gizmos.DrawWireSphere(target.position + offset, 0.5f);
            }
        }
    }
}


