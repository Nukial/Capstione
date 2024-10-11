using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempRenderTarget = RTHandles.Alloc(
                cameraTextureDescriptor.width,
                cameraTextureDescriptor.height,
                depthBufferBits: DepthBits.None,
                colorFormat: GraphicsFormat.R8G8B8A8_SRGB,
                dimension: TextureDimension.Tex2D,
                filterMode: FilterMode.Bilinear,
                name: "_TemporaryColorTexture"
            );

            tempRenderTarget2 = RTHandles.Alloc(
                cameraTextureDescriptor.width,
                cameraTextureDescriptor.height,
                depthBufferBits: DepthBits.None,
                colorFormat: GraphicsFormat.R8G8B8A8_SRGB,
                dimension: TextureDimension.Tex2D,
                filterMode: FilterMode.Bilinear,
                name: "_TemporaryDepthTexture"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Water Volume Pass");

            Blit(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, tempRenderTarget, _material);
            Blit(cmd, tempRenderTarget, renderingData.cameraData.renderer.cameraColorTargetHandle);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(tempRenderTarget);
            RTHandles.Release(tempRenderTarget2);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material = null;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public Settings settings = new Settings();
    private CustomRenderPass _customRenderPass;

    public override void Create()
    {
        _customRenderPass = new CustomRenderPass(settings.material)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_customRenderPass);
    }
}
