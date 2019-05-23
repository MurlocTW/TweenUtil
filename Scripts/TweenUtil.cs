﻿using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Tween
{
    public class TweenUtil : MonoBehaviour
    {
        #region 静态部分

        static TweenUtil instance;

        // static AnimParamHash HashTemp = new AnimParamHash(); 
        public static TweenUtil GetInstance()
        {
            if (instance == null)
            {
                GameObject animGameObject = new GameObject();
                animGameObject.name = "[TweenUtil]";
                instance = animGameObject.AddComponent<TweenUtil>();
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
                else
                {
                    EditorApplication.update += instance.Update;
                }
#else
             DontDestroyOnLoad(instance.gameObject);
#endif
            }

            return instance;
        }

        #region CustomTween

        public static TweenScript CustomTweenFloat(AnimCustomMethodFloat method, float from, float to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.CustomFloat;
            tweenTmp.fromFloat = from;
            tweenTmp.toFloat = to;
            tweenTmp.customMethodFloat = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript CustomTweenVector2(AnimCustomMethodVector2 method, Vector2 from, Vector2 to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.CustomVector2;
            tweenTmp.fromV2 = from;
            tweenTmp.toV2 = to;
            tweenTmp.customMethodV2 = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript CustomTweenVector3(AnimCustomMethodVector3 method, Vector3 from, Vector3 to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.CustomVector3;
            tweenTmp.fromV3 = from;
            tweenTmp.toV2 = to;
            tweenTmp.customMethodV3 = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        #endregion

        #region 功能函数

        /// <summary>
        /// 停止一个动画
        /// </summary>
        /// <param name="tweenData"></param>
        /// <param name="isCallBack">是否触发回调</param>
        public static void StopAnim(TweenScript tweenData, bool isCallBack = false)
        {
            if (isCallBack)
            {
                tweenData.executeCallBack();
            }

            GetInstance().animList.Remove(tweenData);
            StackObjectPool<TweenScript>.PutObject(tweenData);
        }

        public static void FinishAnim(TweenScript tweenData)
        {
            tweenData.currentTime = tweenData.totalTime;
            tweenData.executeUpdate();
            tweenData.executeCallBack();
            GetInstance().animList.Remove(tweenData);
            StackObjectPool<TweenScript>.PutObject(tweenData);
        }

        public static void ClearAllAnim(bool isCallBack = false)
        {
            if (isCallBack)
            {
                for (int i = 0; i < GetInstance().animList.Count; i++)
                {
                    GetInstance().animList[i].executeCallBack();
                    GetInstance().animList.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                GetInstance().animList.Clear();
            }
        }

        #endregion

        #endregion

        #region 实例部分

        public List<TweenScript> animList = new List<TweenScript>();

        public void AddTween(TweenScript tweenScript)
        {
            // 避免同一物体同一类型同时存在两次.
            var a = animList.Find(x =>
                x.animGameObject == tweenScript.animGameObject && x.animType == tweenScript.animType);
            if (a != null)
            {
                animList.Remove(a);
                StackObjectPool<TweenScript>.PutObject(a);
            }

            animList.Add(tweenScript);
        }

        public void Update()
        {
            for (int i = 0; i < animList.Count; i++)
            {
                animList[i].executeUpdate();
                if (animList[i].isDone)
                {
                    TweenScript tweenTmp = animList[i];
                    if (!tweenTmp.AnimReplayLogic())
                    {
                        animList.Remove(tweenTmp);
                        i--;
                        StackObjectPool<TweenScript>.PutObject(tweenTmp);
                    }

                    tweenTmp.executeCallBack(); // todo this is bug.
                }
            }
        }

        #endregion
    }

    #region 枚举与代理声明

    public delegate void AnimCallBack(params object[] arg);

    public delegate void AnimCustomMethodVector3(Vector3 data);

    public delegate void AnimCustomMethodVector2(Vector2 data);

    public delegate void AnimCustomMethodFloat(float data);

    /// <summary>
    /// 动画类型
    /// </summary>
    public enum AnimType
    {
        Position,
        Rotate,
        Scale,
//        LocalPosition,
//        LocalRotate,
        Color, // SpriteRenderer.Color
        Alpha,
        UiColor, // Image.Color  Text.Color
        UiAlpha,
        UiAnchoredPosition,
        UiSize,
        CustomVector3,
        CustomVector2,
        CustomFloat,
        Blink,
    }

    /// <summary>
    /// 插值算法类型
    /// </summary>
    public enum Ease
    {
        Default,
        Linear,
        InBack,
        OutBack,
        InOutBack,
        OutInBack,
        InQuad,
        OutQuad,
        InoutQuad,
        InCubic,
        OutCubic,
        InoutCubic,
        OutInCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        OutInQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        OutInQuint,
        InSine,
        OutSine,
        InOutSine,
        OutInSine,
        InExpo,
        OutExpo,
        InOutExpo,
        OutInExpo,
        OutBounce,
        InBounce,
        InOutBounce,
        OutInBounce,
    }

    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType
    {
        Line,
        Linear,
        CatmullRom,
    }

    public enum LoopType
    {
        Once,
        Loop,
        PingPang,
    }

    #endregion
}