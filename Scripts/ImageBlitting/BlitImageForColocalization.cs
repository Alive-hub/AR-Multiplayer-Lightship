using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

public class BlitImageForColocalization : MonoBehaviour
{
        [SerializeField] private RenderTexture m_RenderTexture;
        private ARCameraBackground m_ARCameraBackground;
        
        private Texture2D _cameraTexture;
        private bool _textureReady = false;
        
        public static event Action<Texture2D> OnTextureRendered; 

        private void Start()
        {
            m_ARCameraBackground = FindObjectOfType<ARCameraBackground>();
            StartGameAR.OnStartSharedSpaceHost += OnStartSharedSpace;
            StartGameAR.OnJoinSharedSpaceClient += OnStartSharedSpace;
        }

        void OnStartSharedSpace()
        {
            UpdateRenderTextureSize();
            BlitCameraImage();
        }
        
        private void BlitCameraImage()
        {
                // Create a new command buffer
                var commandBuffer = new CommandBuffer();
                commandBuffer.name = "AR Camera Background Blit Pass";

                // Get a reference to the AR Camera Background's main texture
                // We will copy this texture into our chosen render texture
                var texture = !m_ARCameraBackground.material.HasProperty("_MainTex") ?
                    null : m_ARCameraBackground.material.GetTexture("_MainTex");

                // Save references to the active render target before we overwrite it
                var colorBuffer = Graphics.activeColorBuffer;
                var depthBuffer = Graphics.activeDepthBuffer;

                // Set  Unity's render target to our render texture
                Graphics.SetRenderTarget(m_RenderTexture);

                // Clear the render target before we render new pixels into it
                commandBuffer.ClearRenderTarget(true, false, Color.clear);

                // Blit the AR Camera Background into the render target
                commandBuffer.Blit(
                    texture,
                    BuiltinRenderTextureType.CurrentActive,
                    m_ARCameraBackground.material);

                // Execute the command buffer
                Graphics.ExecuteCommandBuffer(commandBuffer);

                Graphics.SetRenderTarget(colorBuffer, depthBuffer);

                CopyRenderTextureTo2DTexture();
        }
        
        void UpdateRenderTextureSize()
        {
            var currentRTWidth = m_RenderTexture != null ? m_RenderTexture.width : 0;
            var currentRTHeight = m_RenderTexture != null ? m_RenderTexture.height : 0;


            var newWidth = Screen.width;
            var newHeight = Screen.height;

            if (currentRTWidth != newWidth || currentRTHeight != newHeight)
            {
                if (m_RenderTexture != null)
                {
                    m_RenderTexture.Release();
                }
                m_RenderTexture.width = newWidth;
                m_RenderTexture.height = newHeight;
                m_RenderTexture.depth = 24;
                
                Debug.Log("Changing Render Texture with to " +  m_RenderTexture.width + " and height to " + m_RenderTexture.height);
                
                
                m_RenderTexture.Create();
            }
        }

        public void OnDestroy()
        {
            StartGameAR.OnStartSharedSpaceHost -= OnStartSharedSpace;
            StartGameAR.OnJoinSharedSpaceClient -= OnStartSharedSpace;
        }

        private void CopyRenderTextureTo2DTexture()
        {

            if (_cameraTexture == null || _cameraTexture.width != m_RenderTexture.width ||
                _cameraTexture.height != m_RenderTexture.height)
            {
                _cameraTexture = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.RGBA32,
                    false);
            }

            RenderTexture.active = m_RenderTexture;
            _cameraTexture.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height), 0, 0);
            _cameraTexture.name = "SCAN_IMG";
            _cameraTexture.Apply();
            RenderTexture.active = null;

            OnTextureRendered?.Invoke(_cameraTexture);
        }

}
