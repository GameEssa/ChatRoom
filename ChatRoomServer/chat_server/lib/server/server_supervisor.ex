defmodule Server.Supervisor do
  use Supervisor

  def start_link( opts ) do
    Supervisor.start_link( __MODULE__, :ok, opts )
  end



  def init( :ok ) do
    children = [
      { DynamicSupervisor, strategy: :one_for_one, name: Server.DynamicSupervisor},
      { UserRegistry.Supervisor, []},
      { Server, [port: 8101] }
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end
end
