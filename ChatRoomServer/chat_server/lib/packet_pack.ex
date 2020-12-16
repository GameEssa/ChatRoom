defmodule PacketPack do
  def pack(tag, nil) do
    tagSize = byte_size(tag)
    totalSize = tagSize + 2 * 4;

    int32_bytes(totalSize) <> int32_bytes(tagSize) <> tag
  end

  def pack(tag, content) do
    tagSize = byte_size(tag)
    dataSize = byte_size(content);
    totalSize = tagSize + dataSize + 2 * 4;

    int32_bytes(totalSize) <> int32_bytes(tagSize) <> tag <> content
  end

  defp int32_bytes(number) do
    <<number::big-signed-32>>
  end
end
