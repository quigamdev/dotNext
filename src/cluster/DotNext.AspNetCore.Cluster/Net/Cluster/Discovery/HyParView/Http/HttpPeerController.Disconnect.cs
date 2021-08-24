using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Debug = System.Diagnostics.Debug;

namespace DotNext.Net.Cluster.Discovery.HyParView.Http
{
    using Buffers;
    using IO;
    using IO.Pipelines;

    internal partial class HttpPeerController
    {
        private const string DisconnectMessageType = "Disconnect";

        private async Task ProcessDisconnectAsync(HttpRequest request, HttpResponse response, long payloadLength, CancellationToken token)
        {
            EndPoint sender;
            bool isAlive;

            if (request.BodyReader.TryReadBlock(payloadLength, out var result))
            {
                (sender, isAlive) = DeserializeDisconnectRequest(result.Buffer, out var position);
                request.BodyReader.AdvanceTo(position);
            }
            else
            {
                using var buffer = new PooledBufferWriter<byte>(allocator, payloadLength.Truncate());
                await request.BodyReader.CopyToAsync(buffer, token).ConfigureAwait(false);
                (sender, isAlive) = DeserializeDisconnectRequest(buffer.WrittenMemory);
            }

            await EnqueueDisconnectAsync(sender, isAlive, token).ConfigureAwait(false);
            response.StatusCode = StatusCodes.Status204NoContent;
        }

        private static (EndPoint, bool) DeserializeDisconnectRequest(ref SequenceBinaryReader reader)
            => (reader.ReadEndPoint(), ValueTypeExtensions.ToBoolean(reader.Read<byte>()));

        private static (EndPoint, bool) DeserializeDisconnectRequest(ReadOnlyMemory<byte> buffer)
        {
            var reader = IAsyncBinaryReader.Create(buffer);
            return DeserializeDisconnectRequest(ref reader);
        }

        private static (EndPoint, bool) DeserializeDisconnectRequest(ReadOnlySequence<byte> buffer, out SequencePosition position)
        {
            var reader = IAsyncBinaryReader.Create(buffer);
            var result = DeserializeDisconnectRequest(ref reader);
            position = reader.Position;
            return result;
        }

        protected sealed override async Task DisconnectAsync(EndPoint peer, bool isAlive, CancellationToken token)
        {
            using var request = SerializeDisconnectRequest(isAlive);
            await PostAsync(peer, DisconnectMessageType, request, token).ConfigureAwait(false);
        }

        private MemoryOwner<byte> SerializeDisconnectRequest(bool isAlive)
        {
            Debug.Assert(localNode is not null);

            MemoryOwner<byte> result;
            var writer = new BufferWriterSlim<byte>(64, allocator);

            try
            {
                writer.WriteEndPoint(localNode);
                writer.Add(isAlive.ToByte());

                if (!writer.TryDetachBuffer(out result))
                    result = writer.WrittenSpan.Copy(allocator);
            }
            finally
            {
                writer.Dispose();
            }

            return result;
        }
    }
}