using System.Collections;
using System.Collections.Generic;

public enum TransitionCondition
{

}

public interface IActor
{
    bool Transition(TransitionCondition condition);
}
