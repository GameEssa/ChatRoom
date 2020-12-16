defmodule CommandParse do
  def parse(packet) do
    <<_length::32-integer, tagLength::32-integer , tail::binary>> = packet
    <<tag::binary-size(tagLength) , content::binary>> = tail
    tag = to_string(tag)
    {:ok, tag, content}
  end
end
