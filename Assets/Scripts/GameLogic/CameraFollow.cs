using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test
{
    public class CameraFollow : MonoBehaviour
    {
        //DeepSeekд��
        [Header("��������")]
        [Tooltip("Ҫ�����Ŀ������")]
        public Transform target;          // ����Ŀ��

        [Tooltip("�����ƫ�����������Ŀ��λ�ã�")]
        public Vector3 offset = new Vector3(0f, 2f, -5f); // Ĭ��ƫ����

        [Tooltip("����ƽ��ϵ����ֵԽСԽƽ����")]
        [Range(0.01f, 1f)]
        public float smoothSpeed = 0.125f;

        [Header("��ת����")]
        [Tooltip("�Ƿ����ÿ���Ŀ��")]
        public bool lookAtTarget = true;

        [Tooltip("��תƽ��ϵ��")]
        [Range(0.1f, 5f)]
        public float rotationSmooth = 1f;

        private Vector3 velocity = Vector3.zero;

        void Start()
        {
            // ��ʼλ������
            if (target != null)
            {
                transform.position = target.position + offset;
            }
            else
            {
                Debug.LogWarning("δָ������Ŀ�꣡������ Target��");
            }
        }

        void LateUpdate()
        {
            if (target == null) return;

            // ����Ŀ��λ�ã���ƫ�ƣ�
            Vector3 targetPosition = target.position +
                                    target.right * offset.x +
                                    target.up * offset.y +
                                    target.forward * offset.z;

            // ƽ���ƶ�
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothSpeed);

            // ������ת
            if (lookAtTarget)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmooth * Time.deltaTime);
            }
        }

        // �ڱ༭����ʵʱԤ��ƫ��������ѡ��
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


