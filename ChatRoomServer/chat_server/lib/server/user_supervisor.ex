defmodule UserRegistry.Supervisor do
  use Supervisor

  def start_link(opts) do
    Supervisor.start_link(__MODULE__, :ok, opts)
  end

  def init(:ok) do
    children = [
      { UserRegistry, name: UserRegistry},
      { DynamicSupervisor, strategy: :one_for_one, name: UserRegistry.DynamicSupervisor}
    ]
    Supervisor.init(children, strategy: :one_for_one)
  end
end
