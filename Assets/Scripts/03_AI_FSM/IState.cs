public interface IState
{
    // 1. 当进入这个状态时做什么？ (比如：播放走路动画)
    void OnEnter();

    //2.在这个状态中每帧做什么？ (比如：一直往前走，一直检测距离)
    void OnUpdate();

    // 3. 离开这个状态时做什么？ (比如：停止走路动画，重置参数)
    void OnExit();
}