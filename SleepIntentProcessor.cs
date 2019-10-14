using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;

namespace GrappyAdvanced
{
    public class SleepIntentProcessor : AbstractIntentProcessor
    {
        private const string VERB = "verb";
        private const string SHOULD_WAKE = "shouldWake";
        private const string WAKE_TRIGGER = "wakeTrigger";
        private const string AFTER_WAKE = "afterWake";
        private const string COMB = "comb";
        private const string SLEEP_TIME = "sleepTime";

        public override LexResponse Process(LexEvent lexEvent, ILambdaContext context)
        {
            IDictionary<string, string> slots = lexEvent.CurrentIntent.Slots;
            IDictionary<string, string> sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();

            //if all the values in the slots are empty return the delegate, theres nothing to validate or do.
            if (slots.All(x => x.Value == null))
            {
                return Delegate(sessionAttributes, slots);
            }

            if (string.IsNullOrEmpty(slots[VERB]))
            {
                return ElicitSlot(
                        sessionAttributes,
                        "SleepingIntent",
                        slots,
                        VERB,
                        new LexResponse.LexMessage
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Look at Marvin! What is he doing?"
                        }
                    );
            }

            switch (slots[VERB])
            {
                case "sleeping":
                    return SleepingVerbProcessor(sessionAttributes, slots);
                case "bed":
                    return BedProcessor(sessionAttributes, slots);
                default:
                    return ElicitSlot(
                        sessionAttributes,
                        "SleepingIntent",
                        slots,
                        VERB,
                        new LexResponse.LexMessage
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Look at Marvin! What is he doing?"
                        }
                    );
            }
        }

        private LexResponse BedProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[SHOULD_WAKE]))
            {
                return ElicitSlot(
                    sessionAttributes,
                    "SleepingIntent",
                    slots,
                    SHOULD_WAKE,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Yes! Marvin is in bed, sleeping! Do you think it is time for him to wake up?"
                    });
            }

            if (slots[SHOULD_WAKE] == "y"
                || slots[SHOULD_WAKE] == "t"
                || slots[SHOULD_WAKE] == "yes"
                || slots[SHOULD_WAKE] == "true"
                || slots[SHOULD_WAKE] == "correct"
                || slots[SHOULD_WAKE] == "right")
            {
                return WakeTriggerProcessor(sessionAttributes, slots);
            }

            if (slots[SHOULD_WAKE] == "n"
                || slots[SHOULD_WAKE] == "f"
                || slots[SHOULD_WAKE] == "no"
                || slots[SHOULD_WAKE] == "false"
                || slots[SHOULD_WAKE] == "wrong"
                || slots[SHOULD_WAKE] == "incorrect")
            {
                return ContinueToSleepProcessor(sessionAttributes, slots);
            }

            return ElicitSlot(
                    sessionAttributes,
                    "SleepingIntent",
                    slots,
                    SHOULD_WAKE,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Yes! Marvin is in bed, sleeping! Do you think it is time for him to wake up?"
                    });
        }

        private LexResponse SleepingVerbProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[SHOULD_WAKE]))
            {
                return ElicitSlot(
                    sessionAttributes,
                    "SleepingIntent",
                    slots,
                    SHOULD_WAKE,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Yes!  Marvin is sleeping!  Do you think it is time for him to wake up?"
                    });
            }

            if (slots[SHOULD_WAKE] == "y"
                || slots[SHOULD_WAKE] == "t"
                || slots[SHOULD_WAKE] == "yes"
                || slots[SHOULD_WAKE] == "true"
                || slots[SHOULD_WAKE] == "correct"
                || slots[SHOULD_WAKE] == "right")
            {
                return WakeTriggerProcessor(sessionAttributes, slots);
            }

            if (slots[SHOULD_WAKE] == "n"
                || slots[SHOULD_WAKE] == "f"
                || slots[SHOULD_WAKE] == "no"
                || slots[SHOULD_WAKE] == "false"
                || slots[SHOULD_WAKE] == "wrong"
                || slots[SHOULD_WAKE] == "incorrect")
            {
                return ContinueToSleepProcessor(sessionAttributes, slots);
            }

            return ElicitSlot(
                    sessionAttributes,
                    "SleepingIntent",
                    slots,
                    SHOULD_WAKE,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Yes!  Marvin is sleeping!  Do you think it is time for him to wake up?"
                    });
        }

        private LexResponse WakeTriggerProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[WAKE_TRIGGER]))
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        WAKE_TRIGGER,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Okay, let's wake him up! Can you say, Wake up, Marvin?"
                        });
            }
            else if (slots[WAKE_TRIGGER] == "wake" || slots[WAKE_TRIGGER] == "getup")
            {
                return AfterWakeProcessor(sessionAttributes, slots);
            }

            return ElicitSlot(sessionAttributes,
                    "SleepingIntent",
                    slots,
                    WAKE_TRIGGER,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Okay, let's wake him up! Can you say, Wake up, Marvin?"
                    });
        }

        private LexResponse AfterWakeProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[AFTER_WAKE]))
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        AFTER_WAKE,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Look, you woke him up! He needs to eat breakfast and comb his hair. What should he do first?"
                        });
            }
            else if (slots[AFTER_WAKE] == "comb")
            {
                return SelectCombProcessor(sessionAttributes, slots);
            }

            return ElicitSlot(sessionAttributes,
                    "SleepingIntent",
                    slots,
                    AFTER_WAKE,
                    new LexResponse.LexMessage()
                    {
                        ContentType = MESSAGE_CONTENT_TYPE,
                        Content = "Look, you woke him up! He needs to eat breakfast and comb his hair. What should he do first?"
                    });
        }

        private LexResponse SelectCombProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[COMB]))
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        COMB,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Choose the comb from the image."
                        });
            }
            else
            {
                return Close(sessionAttributes,
                        "Fulfilled",
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Okay, Marvin is ready to go!"
                        });
            }
        }

        private LexResponse ContinueToSleepProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[WAKE_TRIGGER]))
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        WAKE_TRIGGER,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Okay, let's let him sleep. Can you whisper, Shhh?"
                        });
            }

            return SleepingTimeProcessor(sessionAttributes, slots);
        }

        private LexResponse SleepingTimeProcessor(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            if (string.IsNullOrEmpty(slots[SLEEP_TIME]))
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        SLEEP_TIME,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "How long should we let him sleep?"
                        });
            }

            if (slots[SLEEP_TIME] == "8:30")
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        WAKE_TRIGGER,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "Okay, let's wake him up at 8:30, Say Wake up Marvin after that."
                        });
            }

            if (slots[SLEEP_TIME] == "other")
            {
                return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        WAKE_TRIGGER,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "No, we can't do that. Let's wake him up at 8:30. Say Wake up Marvin after that."
                        });
            }

            return ElicitSlot(sessionAttributes,
                        "SleepingIntent",
                        slots,
                        SLEEP_TIME,
                        new LexResponse.LexMessage()
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = "How long should we let him sleep?"
                        });
        }
    }
}
