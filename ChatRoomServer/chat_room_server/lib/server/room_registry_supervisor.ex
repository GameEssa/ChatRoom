defmodule RoomRegistry.Supervisor do
  use Supervisor

  def start_link( opts ) do
    Supervisor.start_link( __MODULE__, :ok, opts )
  end



  def init( :ok ) do
    children = [
      { DynamicSupervisor, strategy: :one_for_one , name: RoomRegistry.Supervisor},
      { RoomRegistry, name: RoomRegistry}
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end
end
