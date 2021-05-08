using System;
using UnityEngine;

/// <summary>
/// Contains extension methods related to <see cref="Timer"/>s.
/// </summary>
namespace InteractionFramework.Runtime
{
    public static class TimerExtensions
    {
        /// <summary>
        /// Attach a timer on to the behaviour. If the behaviour is destroyed before the timer is completed,在行为上附加一个计时器。如果行为在计时器完成前被破坏，
        /// e.g. through a scene change, the timer callback will not execute.通过场景更改，计时器回调将不会执行。
        /// </summary>
        /// <param name="behaviour">The behaviour to attach this timer to.将此计时器附加到的行为。</param>
        /// <param name="duration">The duration to wait before the timer fires.计时器启动前等待的持续时间。</param>
        /// <param name="onComplete">The action to run when the timer elapses.计时器过期时要运行的操作。</param>
        /// <param name="onUpdate">A function to call each tick of the timer. Takes the number of seconds elapsed since
        /// the start of the current cycle.一个函数，用于调用计时器的每个滴答声。取自当前循环开始以来经过的秒数。</param>
        /// <param name="isLooped">Whether the timer should restart after executing.计时器是否应在执行后重新启动。</param>
        /// <param name="useRealTime">Whether the timer uses real-time(not affected by slow-mo or pausing) or
        /// game-time(affected by time scale changes).计时器是否使用实时（不受慢动作或暂停的影响）或游戏时间（受时间刻度变化的影响）。</param>
        public static Timer AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
        Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }
}
