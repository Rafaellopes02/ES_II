document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("gerarRelatorioGeral").addEventListener("click", async () => {
        try {
            const token = localStorage.getItem("jwtToken");

            const eventosResponse = await fetch("/eventos/stats", {
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (!eventosResponse.ok) throw new Error("Erro ao buscar eventos.");

            const data = await eventosResponse.json();
            const eventos = data.eventos;

            const { jsPDF } = window.jspdf;
            const doc = new jsPDF();

            let y = 20;
            let pageNumber = 1;

            function drawHeader(title = "Relatório Geral de Eventos") {
                doc.setFillColor(33, 150, 243); // azul
                doc.rect(0, 0, 210, 20, "F");
                doc.setFontSize(16);
                doc.setTextColor(255, 255, 255);
                doc.setFont(undefined, "bold");
                doc.text(title, 105, 13, null, null, "center");
                doc.setTextColor(0, 0, 0); // voltar ao preto
                y = 30;
            }

            drawHeader();

            for (const evento of eventos) {
                const alturaPrevistaEvento = 60;
                if (y + alturaPrevistaEvento > 270) {
                    doc.addPage();
                    pageNumber++;
                    drawHeader();
                }

                doc.setFontSize(13);
                doc.setFont(undefined, "bold");
                doc.text(evento.nome, 14, y); y += 7;

                doc.setFontSize(11);
                doc.setFont(undefined, "normal");

                const info = [
                    [`Descrição`, evento.descricao],
                    [`Data`, new Date(evento.data).toLocaleDateString("pt-PT")],
                    [`Hora`, evento.hora],
                    [`Local`, evento.local],
                    [`Categoria`, evento.categoriaNome],
                    [`Capacidade`, evento.capacidade],
                    [`Inscritos`, evento.inscritos ?? "N/A"]
                ];

                info.forEach(([label, val]) => {
                    doc.text(`${label}:`, 18, y);
                    doc.text(String(val), 50, y);
                    y += 6;
                });

                // Atividades do evento
                const atividadesResponse = await fetch(`/api/atividades/search-atividades?idevento=${evento.idevento}`, {
                    headers: { "Authorization": `Bearer ${token}` }
                });

                const atividades = atividadesResponse.ok ? await atividadesResponse.json() : [];

                y += 4;
                doc.setFont(undefined, "bold");
                doc.setFontSize(12);
                doc.text("Atividades:", 18, y); y += 6;
                doc.setFont(undefined, "normal");
                doc.setFontSize(11);

                if (atividades.length === 0) {
                    doc.text("Sem atividades registadas.", 22, y); y += 10;
                } else {
                    for (const atv of atividades) {
                        const alturaPrevistaAtividade = 18 + (atv.descricao ? 10 : 0);
                        if (y + alturaPrevistaAtividade > 270) {
                            doc.addPage();
                            pageNumber++;
                            drawHeader();
                        }

                        doc.setFont(undefined, "bold");
                        doc.text(`• ${atv.nome}`, 22, y); y += 5;
                        doc.setFont(undefined, "normal");
                        doc.text(`Data: ${atv.data}    Hora: ${atv.hora}`, 26, y); y += 5;
                        doc.text(`Capacidade: ${atv.quantidademaxima}`, 26, y); y += 5;

                        if (atv.descricao) {
                            const descr = doc.splitTextToSize(`Descrição: ${atv.descricao}`, 160);
                            doc.text(descr, 26, y);
                            y += descr.length * 5;
                        }

                        y += 5;
                    }
                }

                y += 5;
                doc.setDrawColor(180, 180, 180);
                doc.line(14, y, 196, y);
                y += 10;
            }

            doc.save("Relatorio_Geral_Eventos.pdf");
        } catch (err) {
            console.error("Erro ao gerar relatório geral:", err);
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Não foi possível gerar o relatório geral.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    });
});
