defmodule Server.Supervisor do
  use Supervisor

  def start_link( opts ) do
    Supervisor.start_link( __MODULE__, :ok, opts )
  end



  def init( :ok ) do
    children = [
      { RoomRegistry.Supervisor, [] },
      { ClientRegistry.Supervisor, []},
      { Task.Supervisor, name: Server.TaskSupervisor },
      { Server, [port: 8101] }
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end
end
