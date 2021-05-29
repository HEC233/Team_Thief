using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버튼 ui의 마우스, 키보드 상호작용 방식 변환을 위한 코드입니다.
/// </summary>
public interface IUIFocus
{
    /// <summary>
    /// 마우스로 버튼을 선택할때 호출합니다.
    /// </summary>
    void FocusWithMouse();
    /// <summary>
    /// 키보드로 버튼을 선택할때 호출합니다. 이전에 선택중이던 버튼으로 포커스를 바꿔야 합니다.
    /// </summary>
    void FocusWithKeyboard();
}
