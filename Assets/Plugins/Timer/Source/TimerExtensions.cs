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
        /// Attach a timer on to the behaviour. If the behaviour is destroyed before the timer is completed,����Ϊ�ϸ���һ����ʱ���������Ϊ�ڼ�ʱ�����ǰ���ƻ���
        /// e.g. through a scene change, the timer callback will not execute.ͨ���������ģ���ʱ���ص�������ִ�С�
        /// </summary>
        /// <param name="behaviour">The behaviour to attach this timer to.���˼�ʱ�����ӵ�����Ϊ��</param>
        /// <param name="duration">The duration to wait before the timer fires.��ʱ������ǰ�ȴ��ĳ���ʱ�䡣</param>
        /// <param name="onComplete">The action to run when the timer elapses.��ʱ������ʱҪ���еĲ�����</param>
        /// <param name="onUpdate">A function to call each tick of the timer. Takes the number of seconds elapsed since
        /// the start of the current cycle.һ�����������ڵ��ü�ʱ����ÿ���δ�����ȡ�Ե�ǰѭ����ʼ����������������</param>
        /// <param name="isLooped">Whether the timer should restart after executing.��ʱ���Ƿ�Ӧ��ִ�к�����������</param>
        /// <param name="useRealTime">Whether the timer uses real-time(not affected by slow-mo or pausing) or
        /// game-time(affected by time scale changes).��ʱ���Ƿ�ʹ��ʵʱ����������������ͣ��Ӱ�죩����Ϸʱ�䣨��ʱ��̶ȱ仯��Ӱ�죩��</param>
        public static Timer AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
        Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }
}
