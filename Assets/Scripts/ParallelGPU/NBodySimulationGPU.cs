using UnityEngine;

public class NBodySimulationGPU : MonoBehaviour
{
    [SerializeField] Material bodyMat;
    [SerializeField] int numberOfBodies = 625;
    [SerializeField] ComputeShader compShader;
    [SerializeField] float quadSize = 1;

    readonly Bounds renderBounds = new Bounds(Vector3.zero, Vector3.one * 100000);//for frustrum culling
    const float DELTATIME = 0.05f;
    const float distanceThreshold = 0.05f;
    const int numThreads = 1024;
    const int gridSize = 175;
    const int vertexCount = 6;
    
    ComputeBuffer quadVerticesBuffer;
    ComputeBuffer bodiesBuffer;
    int kernel;
    int numThreadGroups;

    // Start is called before the first frame update
    void Start()
    {
        SetupRenderShader();
        SetupComputeShader();
    }

    void SetupComputeShader()
    {
        numberOfBodies = gridSize * gridSize;
        Body[] bodies = new Body[numberOfBodies];

        int firstGalaxyGridSize = 70;
        int secondGalaxyGridSize = gridSize - firstGalaxyGridSize;
        int k = 0;

        //create first galaxy
        for (int i = 0; i < firstGalaxyGridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(-gridSize / 2 + i, -gridSize / 2 + j) * 1.5f;
                bodies[k] = new Body();
                bodies[k].pos = position + Vector3.left * 500;
                bodies[k].vel = Random.insideUnitSphere + Vector3.Cross(position, Vector3.back).normalized * 3 + Vector3.right * 2f + Vector3.up*1.5f;
                bodies[k++].mass = 3e-1f;
            }
        }

        //create second galaxy
        for (int i = 0; i < secondGalaxyGridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = Quaternion.Euler(90,0,0) * Random.insideUnitCircle * gridSize* 0.5f * 1.5f;
                bodies[k] = new Body();
                bodies[k].pos = position+ Vector3.right * 500;
                bodies[k].vel = Random.insideUnitSphere + Vector3.Cross(position, Vector3.up).normalized * 3 + Vector3.left * 2f;
                bodies[k++].mass = 1e-1f;
            }
        }

        //Instantiate compute shader
        kernel = compShader.FindKernel("NBodyMain");
        bodiesBuffer = new ComputeBuffer(bodies.Length, 10 * sizeof(float));//Vector3 has 3floats => 3float * 3Vector3 + 1float = 10 float
        bodiesBuffer.SetData(bodies);

        Shader.SetGlobalBuffer("bodies", bodiesBuffer);//global buffer because rendering shader also uses this buffer
        compShader.SetBuffer(kernel, "bodies", bodiesBuffer);
        compShader.SetFloat("DELTATIME", DELTATIME);
        compShader.SetFloat("distanceThreshold", distanceThreshold);
        compShader.SetInt("numBodies", numberOfBodies);

        numThreadGroups = Mathf.CeilToInt((float)numberOfBodies / numThreads);
    }

    void SetupRenderShader()
    {
        //Create our mesh vertices
        Vector3[] quadVertices = new Vector3[]
        {
            new Vector3(-0.5f, 0.5f)*quadSize,
            new Vector3(0.5f, 0.5f)*quadSize,
            new Vector3(0.5f, -0.5f)*quadSize,
            new Vector3(0.5f, -0.5f)*quadSize,
            new Vector3(-0.5f, -0.5f)*quadSize,
            new Vector3(-0.5f, 0.5f)*quadSize
        };

        //set our vertices in the vram (GPU memory)
        quadVerticesBuffer = new ComputeBuffer(quadVertices.Length, 3 * sizeof(float));
        quadVerticesBuffer.SetData(quadVertices);
        bodyMat.SetBuffer("quadVertices", quadVerticesBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        compShader.Dispatch(kernel, numThreadGroups, 1, 1);
        Graphics.DrawProcedural(bodyMat, renderBounds, MeshTopology.Triangles, vertexCount, numberOfBodies);
    }

    //Release GPU memory when stopping the simulation
    private void OnDestroy()
    {
        bodiesBuffer?.Release();
        quadVerticesBuffer?.Release();
    }
}