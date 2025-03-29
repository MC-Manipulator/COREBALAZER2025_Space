using UnityEngine;
using System.Collections.Generic;

public class SpaceshipController : MonoBehaviour
{
    [Header("输入缓冲引用")]
    public InputBuffer inputBuffer;
    [Header("推进")]
    public float thrustImpulse = 0.6f; // 反推力强度
    public float inputBufferTime = 0.2f;
    private Rigidbody2D rb;
    private List<float> _inputTimestamps = new List<float>(); // 存储有效输入时间戳

/*    [Header("轨道预测")]
    public LineRenderer lineRenderer;
    public int predictionPoints = 10;     // 预测点数
    public float predictionTimeStep = 0.02f; // 每步模拟时间
    public float maxPredictionLength = 20f; // 虚线最大长度*/

    private PlanetGravity currentPlanet; // 当前所在星球

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
/*        // 初始化 LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
*//*        linerenderer.material = resources.load<material>("dashedline");
        linerenderer.texturemode = linetexturemode.tile;*//*
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // 初始隐藏*/
    }

    void Update()
    {
        inputBuffer.ClearExpiredInputs();
        // 发射激光（按F键）
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 这里添加激光生成逻辑
        }

/*        if (Input.GetKey(KeyCode.LeftShift))
        { // 按住Shift显示预测线
            PredictTrajectory();
        }
        else
        {
            //lineRenderer.positionCount = 0; // 隐藏线条
        }*/

    }

    void FixedUpdate()
    {
        if (inputBuffer.HasValidInput())
        {
            // 应用所有未处理的输入
            rb.velocity += (Vector2)transform.up * (thrustImpulse / rb.mass) * inputBuffer.GetInputCount();
            inputBuffer.ClearInputs();
        }
        //Debug.Log($"当前速度: {rb.velocity.magnitude}");
    }

/*    void PredictTrajectory()
    {
        List<Vector3> points = new List<Vector3>();

        // 克隆飞船（禁用碰撞器和渲染）
        GameObject simulatedObj = Instantiate(
            rb.gameObject,
            rb.position,
            Quaternion.identity
        );
        simulatedObj.GetComponent<Collider2D>().enabled = false;
        simulatedObj.GetComponent<SpriteRenderer>().enabled = false;
        Rigidbody2D simulatedRB = simulatedObj.GetComponent<Rigidbody2D>();

        // 同步初始状态
        simulatedRB.velocity = rb.velocity;
        simulatedRB.gravityScale = 0;
        float simulatedMass = 0.5f; // 获取质量

        // 记录克隆时的输入状态
        bool isThrusting = Input.GetKey(KeyCode.Space);

        Vector2 currentVelocity = simulatedRB.velocity;
        Vector2 currentPosition = simulatedRB.position;

        for (int i = 0; i < predictionPoints; i++)
        {
            // 计算加速度
            Vector2 acceleration = Vector2.zero;

            // 引力加速度（单位：m/s²）
            if (currentPlanet != null)
            {
                Vector2 planetPos = (Vector2)currentPlanet.transform.position;
                Vector2 gravityDir = (planetPos - currentPosition).normalized;
                acceleration += gravityDir * currentPlanet.gravityForce;
            }

            // 推进加速度（单位：m/s²）
            if (isThrusting)
            {
                Vector2 thrustDir = (Vector2)simulatedRB.transform.up;
                acceleration += thrustDir * (thrustImpulse / simulatedMass); // 冲量转加速度
            }

            // 更新速度（v += a × Δt）
            currentVelocity += acceleration * predictionTimeStep;
            Debug.DrawRay(currentPosition, currentVelocity, Color.green);

            // 更新位置（p += v × Δt）
            currentPosition += currentVelocity * predictionTimeStep;

            points.Add(currentPosition);

            // 超过最大长度则退出
            if (Vector2.Distance(rb.position, currentPosition) > maxPredictionLength) break;

        }

        Destroy(simulatedObj);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    
    }*/

/*    float CalculatePathLength()
    {
        float length = 0;
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            length += Vector3.Distance(
                lineRenderer.GetPosition(i - 1),
                lineRenderer.GetPosition(i)
            );
        }
        return length;
    }*/

    // 当进入触发器时
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlanetGravity planet = other.GetComponent<PlanetGravity>();
        if (planet != null)
        {
            currentPlanet = planet; // 更新当前星球
        }
    }

    // 当离开触发器时
    private void OnTriggerExit2D(Collider2D other)
    {
        PlanetGravity planet = other.GetComponent<PlanetGravity>();
        if (planet != null && planet == currentPlanet)
        {
            currentPlanet = null; // 离开当前星球区域
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PlanetGravity planet = other.GetComponent<PlanetGravity>();
        if (planet == null) return;

        if (currentPlanet == null)
        {
            currentPlanet = planet;
        }
        else
        {
            // 比较距离，选择更近的
            float newDistance = Vector2.Distance(transform.position, planet.transform.position);
            float oldDistance = Vector2.Distance(transform.position, currentPlanet.transform.position);
            if (newDistance < oldDistance)
            {
                currentPlanet = planet;
            }
        }
    }

    // 获取当前星球的优化方法
    public PlanetGravity GetCurrentPlanet()
    {
        return currentPlanet;
    }

}