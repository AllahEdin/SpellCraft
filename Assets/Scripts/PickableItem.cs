using System;
using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class PickableItem : CustomNetworkBehaviour
    {
        [SerializeField] private TextView _textView;

        private bool IsReady =>
            _isKeyReady && _isDescriptorReady && _isStateReady;
        private bool _isStateReady = true;
        private bool _isKeyReady = false;
        private bool _isDescriptorReady = false;
        private string _key;
        private NetworkObjectDescriptor _descriptor;

        public event Action<PickableItem, PlayerSlot> PickedByPlayer;

        /// <summary>
        /// Server and client
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _key = value;
                    _isKeyReady = true;
                    _textView.SetText(value);
                }
            }
        }

        /// <summary>
        /// Server only
        /// </summary>
        public NetworkObjectDescriptor Descriptor
        {
            get => _descriptor;
            set
            {
                _descriptor = value;
                _isDescriptorReady = true;
            }
        }

        public override void OnStartClient()
        {
            GetComponent<Collider>().enabled = false;
            base.OnStartClient();
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider col)
        {
            if (!IsReady)
                return;

            var player = col.transform.GetComponent<Player>();
            if (player != null)
            {
                if (netIdentity.connectionToClient == player.netIdentity.connectionToClient)
                {
                    PickedByPlayer?.Invoke(this, player.Slot);
                    _isStateReady = false;
                }
            }
        }

        [ClientRpc]
        public void RpcSetKey(string key)
        {
            Key = key;
        }
    }
}