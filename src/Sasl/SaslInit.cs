// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Amqp.Sasl
{
    using System;
    using System.Text;
    using Microsoft.Azure.Amqp.Encoding;
    using Microsoft.Azure.Amqp.Framing;

    /// <summary>
    /// Initiate sasl exchange.
    /// </summary>
    public sealed class SaslInit : Performative
    {
        /// <summary>
        /// The descriptor name.
        /// </summary>
        public static readonly string Name = "amqp:sasl-init:list";
        /// <summary>
        /// The descriptor code.
        /// </summary>
        public static readonly ulong Code = 0x0000000000000041;
        const int Fields = 3;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public SaslInit() : base(Name, Code) { }

        internal override int FieldCount
        {
            get { return Fields; }
        }

        /// <summary>
        /// Gets or sets the selected SASL mechanism.
        /// </summary>
        public AmqpSymbol Mechanism { get; set; }

        /// <summary>
        /// Gets or sets the initial security response data.
        /// </summary>
        public ArraySegment<byte> InitialResponse { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets a string representing the object.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("sasl-init(");
            int count = 0;
            this.AddFieldToString(this.Mechanism.Value != null, sb, "mechanism", this.Mechanism, ref count);
            this.AddFieldToString(this.InitialResponse.Array != null, sb, "initial-response", this.InitialResponse, ref count);
            this.AddFieldToString(this.HostName != null, sb, "host-name", this.HostName, ref count);
            sb.Append(')');
            return sb.ToString();
        }

        internal override void EnsureRequired()
        {
            if (this.Mechanism.Value == null)
            {
                throw new AmqpException(AmqpErrorCode.InvalidField, "mechanism");
            }
        }

        internal override void OnEncode(ByteBuffer buffer)
        {
            AmqpCodec.EncodeSymbol(this.Mechanism, buffer);
            AmqpCodec.EncodeBinary(this.InitialResponse, buffer);
            AmqpCodec.EncodeString(this.HostName, buffer);
        }

        internal override void OnDecode(ByteBuffer buffer, int count)
        {
            if (count-- > 0)
            {
                this.Mechanism = AmqpCodec.DecodeSymbol(buffer);
            }

            if (count-- > 0)
            {
                this.InitialResponse = AmqpCodec.DecodeBinary(buffer);
            }

            if (count-- > 0)
            {
                this.HostName = AmqpCodec.DecodeString(buffer);
            }
        }

        internal override int OnValueSize()
        {
            int valueSize = 0;

            valueSize += AmqpCodec.GetSymbolEncodeSize(this.Mechanism);
            valueSize += AmqpCodec.GetBinaryEncodeSize(this.InitialResponse);
            valueSize += AmqpCodec.GetStringEncodeSize(this.HostName);

            return valueSize;
        }
    }
}
