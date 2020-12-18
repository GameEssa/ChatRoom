defmodule MessageTag do
  use EnumType

  defenum Tag do
    value None, "None"

    value LoginTag, "Net.Message.Login"

    value CreateRoomTag , "Net.Message.CreateRoom"

    value GetAllRoomTag , "Net.Message.GetAllRoom"

    value EnterRoomTag , "Net.Message.EnterRoom"

    value ExitRoomTag , "Net.Message.ExitRoom"

    value SendDialogueTag , "Net.Message.SendDialogue"

    value ReceiveDialogueTag , "Net.Message.ReceiveDialogue"
    default Blue
  end


end
