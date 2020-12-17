defmodule ServerTest do
  use ExUnit.Case
  doctest Server

  test "greets the world" do
    assert Server.say() == :read
  end
end
