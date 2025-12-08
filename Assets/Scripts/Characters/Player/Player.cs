using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    //单例，方便全局访问
    public static Player Instance { get; private set; }

    //集中管理所有子系统的引用
    public PlayerStateMachine stateMachine { get;private set; }
    public PlayerSkillManager skillManager { get; private set; }
    public PlayerWeaponManager weaponManager { get; private set; }

    public HealthController healthController { get; private set; }
    public CharacterAnimation characterAnimation { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public LineRenderer line {  get; private set; }

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        //获取自身组件
        stateMachine = GetComponent<PlayerStateMachine>();
        skillManager =  GetComponent<PlayerSkillManager>();
        weaponManager = GetComponent<PlayerWeaponManager>();

        healthController = GetComponent<HealthController>();
        characterAnimation = GetComponent<CharacterAnimation>();
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();

        //默认设置
        agent.stoppingDistance = 0.1f;
        if (line != null) line.positionCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
