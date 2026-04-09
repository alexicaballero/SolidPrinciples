namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: El generador de informes SÓLO orquesta el flujo de trabajo del informe.
/// Delega recuperación de datos, formateo y entrega a colaboradores enfocados.
///
/// Compáralo con Problem/CommunityReportService.cs que hacía los tres en una clase.
/// </summary>
/// <remarks>
/// Cada colaborador tiene una responsabilidad única:
/// - IReportDataProvider → obtiene datos
/// - IReportFormatter → convierte datos a un formato específíco
/// - IReportSender → entrega el informe
///
/// Beneficios:
/// - Prueba el formateo sin una base de datos
/// - Prueba la entrega sin construir un informe real
/// - Agrega formato PDF creando PdfReportFormatter — sin cambios de código existente
/// </remarks>
public sealed class CommunityReportGenerator(
    IReportDataProvider dataProvider,
    IReportFormatter formatter,
    IReportSender sender)
{
  /// <summary>
  /// Genera y envía un informe de comunidad delegando cada paso.
  /// </summary>
  public async Task GenerateAndSendAsync(Guid communityId, string recipientEmail, CancellationToken ct = default)
  {
    // CORRECTO: Paso 1 — Delega recuperación de datos (responsabilidad del proveedor de datos)
    var sessions = await dataProvider.GetSessionsByCommunityAsync(communityId, ct);

    // CORRECTO: Paso 2 — Delega formateo (responsabilidad del formateador)
    var content = formatter.Format(sessions);

    // CORRECTO: Paso 3 — Delega entrega (responsabilidad del remitente)
    await sender.SendAsync(recipientEmail, "Community Session Report", content, ct);
  }
}
