﻿using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public abstract class HUDFlicker
    {
        public enum AlphaDirection
        {
            None,
            Decreasing,
            Increasing
        }
        protected AlphaDirection alphaDirection;
        protected float chanceReverseState = 0;
        protected float alphaSpeed;
        protected float alphaUpper;
        protected float alphaLower;
        protected float initialAlpha;
        public float AlphaValue { get; protected set; }
        protected int reversalCount = 0;
        protected int reversalCountThreshold;
        public bool IsBurnedOut { get; set; }

        public HUDFlicker()
        {
            Init();
        }
        public abstract void Init();
        public virtual void InitAlphaDirection(AlphaDirection direct)
        {
            if (direct == AlphaDirection.Decreasing)
                AlphaValue = alphaUpper;
            else if (direct == AlphaDirection.Increasing)
                AlphaValue = alphaLower;

            alphaDirection = direct;
        }
        public virtual void CheckReverseAlphaDirection(bool randomReversal = true)
        {
            // Alpha Direction Reversal Check
            if ((alphaDirection == AlphaDirection.Increasing && AlphaValue >= alphaUpper) ||
                (alphaDirection == AlphaDirection.Decreasing && AlphaValue <= alphaLower))
                ReverseAlphaDirection();
            else if (randomReversal && AlphaValue >= alphaLower && AlphaValue <= alphaUpper)
                RandomlyReverseAlphaDirection();
        }
        protected void SetAlphaValue()
        { 
            // increment alpha depending on State
            if (alphaDirection == AlphaDirection.Decreasing)
            {
                AlphaValue -= alphaSpeed * Time.deltaTime;
            }
            else if (alphaDirection == AlphaDirection.Increasing)
            {
                AlphaValue += alphaSpeed * Time.deltaTime;
            }
            else if (alphaDirection == AlphaDirection.None)
            {
                AlphaValue = 0;
            }
            AlphaValue = Mathf.Clamp(AlphaValue, 0, 1);
        }
        public virtual void Cycle()
        {
            if (!IsBurnedOut)
            {
                //InitAlphaDirection();
                CheckReverseAlphaDirection();
                SetAlphaValue();
            }
        }

        public virtual void Reset()
        {
            IsBurnedOut = false;
            reversalCount = 0;
        }
 
        protected void RandomlyReverseAlphaDirection()
        {
            const float minChance = -0.01f; // negative so it takes a little while before it can reverse at least.
            const float chanceStep = 0.008f;

            if (alphaDirection == AlphaDirection.None)
                return;

            // chance of reversal grows over time.
            chanceReverseState += chanceStep * Time.deltaTime;

            // randomly reverse alpha
            if (Random.Range(0f, 1f) < chanceReverseState)
            {
                // chance gets reset each time it's randomly reversed.
                chanceReverseState = minChance;
                ReverseAlphaDirection();
            }
        }
        private void ReverseAlphaDirection()
        {
            if (alphaDirection == AlphaDirection.Increasing)
                alphaDirection = AlphaDirection.Decreasing;
            else if (alphaDirection == AlphaDirection.Decreasing)
                alphaDirection = AlphaDirection.Increasing;
            reversalCount++;
            //Debug.Log("Reversed AlphaState");
        }
    }
}