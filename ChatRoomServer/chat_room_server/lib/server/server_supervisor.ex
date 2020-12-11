defmodule Server.Supervisor do
  use Supervisor

  def start_link( opts ) do
    Supervisor.start_link( __MODULE__, :ok, opts )
  end



  def init( :ok ) do
    children = [
      { DynamicSupervisor, strategy: :one_for_one, name: Server.DynamicSupervisor },
      #Supervisor.child_spec( {Task, fn -> Server.start_link([port: 8101]) end } , restart: :permanent , shutdown: 10_000)
      { Server, [port: 8101] }
    ]

    Supervisor.init( children, strategy: :one_for_one )
  end
end
