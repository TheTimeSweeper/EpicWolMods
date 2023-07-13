using System;
using BepInEx;
using UnityEngine;

namespace sillyTM_blobs
{
    [BepInPlugin("swepersecreet.sillyblobs", "nemblob", "0.0.1")]
    public class blobzblobzblobzmod : BaseUnityPlugin {

        float sped;
        
        void Awake()
        {
            #region help
            Debug.Log("\nWe're no strangers to love\n" +
"You know the rules and so do I                      \n" +
"A full commitment's what I'm thinking of            \n" +
"You wouldn't get this from any other guy            \n" +
"                                                    \n" +
"I just wanna tell you how I'm feeling               \n" +
"Gotta make you understand                           \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n" +
"                                                    \n" +
"We've known each other for so long                  \n" +
"Your heart's been aching, but                       \n" +
"You're too shy to say it                            \n" +
"Inside, we both know what's been going on           \n" +
"We know the game and we're gonna play it            \n" +
"                                                    \n" +
"And if you ask me how I'm feeling                   \n" +
"Don't tell me you're too blind to see               \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n" +
"                                                    \n" +
"(Ooh, give you up)                                  \n" +
"(Ooh, give you up)                                  \n" +
"Never gonna give, never gonna give                  \n" +
"(Give you up)                                       \n" +
"Never gonna give, never gonna give                  \n" +
"(Give you up)                                       \n" +
"                                                    \n" +
"We've known each other for so long                  \n" +
"Your heart's been aching, but                       \n" +
"You're too shy to say it                            \n" +
"Inside, we both know what's been going on           \n" +
"We know the game and we're gonna play it            \n" +
"                                                    \n" +
"I just wanna tell you how I'm feeling               \n" +
"Gotta make you understand                           \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n" +
"                                                    \n" +
"Never gonna give you up                             \n" +
"Never gonna let you down                            \n" +
"Never gonna run around and desert you               \n" +
"Never gonna make you cry                            \n" +
"Never gonna say goodbye                             \n" +
"Never gonna tell a lie and hurt you                 \n"
);
            #endregion

            sped = Config.Bind("hello", "blobsped", 69, "sped of blob").Value;

            On.BlobRoller.BlobRollState.FixedUpdate += this.BlobRollState_FixedUpdate;
        }

        private void BlobRollState_FixedUpdate(On.BlobRoller.BlobRollState.orig_FixedUpdate orig, BlobRoller.BlobRollState self)
        {
            orig(self);
            if (!self.rollStarted || self.targetHit || self.targetTrans == null)
            {
                return;
            }
                self.parent.movement.MoveToMoveVector(sped, false);
        }
    }
}