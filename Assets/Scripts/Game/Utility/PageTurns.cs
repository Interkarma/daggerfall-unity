using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility
{
    public class PageTurns
    {
        const float pageTurnDelay = 0.35f;
        const SoundClips pageTurnSoundclip = SoundClips.PageTurn;

        public class PageSpan
        {
            readonly int firstMessage;
            readonly int lastMessage;

            public PageSpan(int firstMessage, int lastMessage)
            {
                this.firstMessage = firstMessage;
                this.lastMessage = lastMessage;
            }

            public bool overlaps(PageSpan otherPage)
            {
                return otherPage.firstMessage <= lastMessage && otherPage.lastMessage >= firstMessage;
            }
        }

        private float lastPageTurn = 0f;

        private PageSpan currentPageSpan = null;
        private PageSpan fixedPageSpan = null;

        public PageTurns()
        {
        }

        public void Reset()
        {
            currentPageSpan = null;
            fixedPageSpan = null;
        }

        public void SetPage(int firstMessage, int lastMessage)
        {
            currentPageSpan = new PageSpan(firstMessage, lastMessage);
        }

        public void Play()
        {
            if (fixedPageSpan == null)
                fixedPageSpan = currentPageSpan;
            else if (!fixedPageSpan.overlaps(currentPageSpan))
            {
                fixedPageSpan = currentPageSpan;
                ThrottledPageTurnSound();
            }
        }

        // Prevent annoying effect when quickly flipping pages (say, mouse wheel)
        private void ThrottledPageTurnSound()
        {
            if (Time.realtimeSinceStartup >= lastPageTurn + pageTurnDelay)
            {
                lastPageTurn = Time.realtimeSinceStartup;
                DaggerfallUI.Instance.PlayOneShot(pageTurnSoundclip);
            }
        }
    }
}
