async function getUserIdAndType() {
    try {
        const response = await fetch("/api/auth/me", {
            credentials: "include"
        });

        if (!response.ok) throw new Error("Não autenticado");

        const data = await response.json();
        return {
            userId: parseInt(data.userId),
            userType: parseInt(data.tipoUtilizador)
        };
    } catch (err) {
        console.error("Erro ao obter dados do utilizador:", err);
        return { userId: null, userType: null };
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    const auth = await getUserIdAndType();

    if (!auth.userId || !auth.userType) {
<<<<<<< HEAD

=======
>>>>>>> 53941121e5fbf09b0cf9829908b7146c9652dc58
        auth.userId = null;
        auth.userType = null;
    }

    try {
        const response = await fetch("/eventos/stats", {
            credentials: "include"
        });

<<<<<<< HEAD
        const data = await response.json();

        if (response.ok || (!auth.userId && !auth.userType)) {
            renderizarEventos(data.eventos ?? [], auth.userId, auth.userType);
        } else if (response.status === 401 && auth.userId) {
            await Swal.fire({
                icon: 'warning',
                title: 'Sessão Expirada',
                text: 'Por favor, inicie sessão novamente.',
                timer: 2000,
                showConfirmButton: false
            });
            window.location.href = "/login";
=======
        if (response.ok) {
            const data = await response.json();
            renderizarEventos(data.eventos, auth.userId, auth.userType); // Aqui sim!
        } else if (response.status === 401) {
                const data = await response.json();
                renderizarEventos(data.eventos ?? [], null, null);
>>>>>>> 53941121e5fbf09b0cf9829908b7146c9652dc58
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao buscar eventos.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    } catch (error) {
        console.error("Erro ao carregar eventos:", error);
        await Swal.fire({
            icon: 'error',
            title: 'Erro de ligação',
            text: 'Erro de comunicação com o servidor.',
            timer: 2000,
            showConfirmButton: false
        });
    }


    carregarCategorias("searchCategoria");
    carregarCategorias("eventCategory");
    carregarCategorias("editEventCategory");

});



function renderizarEventos(eventos, userId, userType) {

    const eventList = document.getElementById("eventList");
    eventList.innerHTML = "";

    eventos.forEach(evento => {
        const formattedDate = new Date(evento.data).toLocaleDateString('pt-PT');
        const eventoDateTime = new Date(`${evento.data}T${evento.hora}`);
        const eventoPassado = eventoDateTime < new Date();
        const inscritos = evento.inscritos ?? 0;
        const percentagem = Math.min(100, Math.round((inscritos / evento.capacidade) * 100));
        const corBarra = percentagem === 100 ? "bg-primary" : "bg-info";

        const barra = `
            <div class="progress">
                <div class="progress-bar ${corBarra}" style="width: ${percentagem}%">
                    ${percentagem}% da capacidade
                </div>
            </div>
        `;

        let botoesPrincipais = `
    <button class="btn btn-outline-primary" onclick="carregarDetalhesEvento(${evento.idevento})">
        Detalhes
    </button>
`;

        if ((evento.eorganizador && evento.idutilizador === userId) || userType === 1) {
            botoesPrincipais += `
        <button class="btn btn-outline-secondary" onclick="gerarRelatorioEvento(${evento.idevento})">
            Gerar Relatório
        </button>
    `;
        }
        botoesPrincipais += `
    <div id="detalhes-${evento.idevento}" style="display: none;"></div>
`;

        if ((evento.eorganizador && evento.idutilizador === userId) || userType === 1) {
            botoesPrincipais += `
                <button 
                    class="btn btn-outline-primary editar-btn"
                    data-id="${evento.idevento}"
                    data-nome="${evento.nome}"
                    data-descricao="${evento.descricao}"
                    data-data="${evento.data}"
                    data-hora="${evento.hora}"
                    data-local="${evento.local}"
                    data-capacidade="${evento.capacidade}"
                    data-categoria="${evento.idcategoria}">
                    Editar
                </button>
                <button 
                    class="btn btn-outline-danger eliminar-btn"
                    data-id="${evento.idevento}">
                    Eliminar
                </button>
            `;
        }

        let botoesSecundarios = "";

        if (
            (evento.inscrito && userType === 3) ||
            (evento.eorganizador && evento.idutilizador === userId) ||
            userType === 1
        ) {
            botoesSecundarios += `
                <button 
                    class="btn btn-outline-success Entrar-btn me-2"
                    onclick="window.location.href='/Eventos/${evento.idevento}'">
                    Entrar
                </button>
            `;
        }

        if (
            userType === 3 &&
            evento.inscrito &&
            !eventoPassado &&
            !(evento.eorganizador && evento.idutilizador === userId)
        ) {
            botoesSecundarios += `
                <button 
                    class="btn btn-outline-warning cancelar-btn"
                    data-id="${evento.idevento}">
                    Cancelar inscrição
                </button>
            `;
        }

        if (!evento.inscrito && userType === 3 && inscritos < evento.capacidade && !eventoPassado) {
            botoesSecundarios += `
                <button 
                    class="btn btn-outline-success inscrever-btn"
                    data-id="${evento.idevento}">
                    Comprar Ingresso
                </button>
    `;
        }

        const card = document.createElement("div");
        card.className = "card mb-3 p-3 event-card";
        card.dataset.categoria = evento.idcategoria;  // existente
        // Novos data-attributes para filtragem
        card.dataset.inscrito = evento.inscrito;
        card.dataset.eorganizador = evento.eorganizador;

        card.innerHTML = `
            <h5 class="card-title">${evento.nome}</h5>
            <p class="card-text">${evento.descricao}</p>
            <p class="card-text">
                <small class="text-muted">${formattedDate} · ${evento.hora} · ${evento.local}</small>
            </p>
            ${barra}
            <div class="d-flex justify-content-between align-items-center mt-3 flex-wrap gap-2">
                <div>${botoesPrincipais}</div>
                <div class="ms-auto">${botoesSecundarios}</div>
            </div>
        `;

        eventList.appendChild(card);
    });
}




function carregarDetalhesEvento(id) {
    const detalhesDiv = document.getElementById(`detalhes-${id}`);

    if (detalhesDiv.style.display === "block") {
        detalhesDiv.style.display = "none";
        return;
    }

    fetch(`/api/eventos/detalhes/${id}`, {
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
        }
    })
        .then(res => {
            if (!res.ok) {
                throw new Error("Erro ao buscar detalhes do evento.");
            }
            return res.json();
        })
        .then(data => {
            const dataOriginal = data.data.split("T")[0];
            const horaOriginal = data.hora;

            detalhesDiv.innerHTML = `
                <div class="card card-body mt-2">
                    <p><strong>Nome:</strong> ${data.nome}</p>
                    <p><strong>Descrição:</strong> ${data.descricao}</p>
                    <p><strong>Data:</strong> ${dataOriginal} ${horaOriginal}</p>
                    <p><strong>Local:</strong> ${data.local}</p>
                    <p><strong>Categoria:</strong> ${data.categoriaNome}</p>
                    <p><strong>Capacidade:</strong> ${data.capacidade}</p>
                    <p><strong>Inscritos:</strong> ${data.inscritos ?? "N/A"}</p>
                </div>
            `;
            detalhesDiv.style.display = "block";
        })
        .catch(async error => {
            console.error("Erro ao carregar detalhes:", error);
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao carregar os detalhes do evento.',
                timer: 2000,
                showConfirmButton: false
            });
        });
}

async function carregarCategorias(selectId) {
    try {
        const response = await fetch("/api/categorias");
        if (response.ok) {
            const categorias = await response.json();
            const select = document.getElementById(selectId);
            if (!select) return;

            select.innerHTML = '<option value="">Selecionar...</option>';

            categorias.forEach(categoria => {
                const option = document.createElement("option");
                option.value = categoria.idcategoria;
                option.textContent = categoria.nome;
                select.appendChild(option);
            });
        } else {
            console.warn("Erro ao carregar categorias");
        }
    } catch (err) {
        console.error("Erro ao carregar categorias:", err);
    }
}
async function gerarRelatorioEvento(id) {
    try {
        const token = localStorage.getItem("jwtToken");

        // Buscar dados do evento
        const responseEvento = await fetch(`/api/eventos/detalhes/${id}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (!responseEvento.ok) throw new Error("Erro ao buscar detalhes do evento.");
        const data = await responseEvento.json();

        // Buscar atividades
        const responseAtividades = await fetch(`/api/atividades/search-atividades?idevento=${id}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        const atividades = responseAtividades.ok ? await responseAtividades.json() : [];

        // Buscar ingressos para cálculo de receita
        const responseIngressos = await fetch(`/api/ingressos/por-evento/${id}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        const ingressos = responseIngressos.ok ? await responseIngressos.json() : [];

        let receitaTotal = 0;
        let vendidos = 0;

        // Cálculo da receita com validações
        ingressos.forEach(ing => {
            const definidas = parseInt(ing.quantidadedefinida);
            const atuais = parseInt(ing.quantidadeatual);
            const preco = parseFloat(ing.preco);

            if (!isNaN(definidas) && !isNaN(atuais) && !isNaN(preco)) {
                vendidos = definidas - atuais;
                const receita = vendidos * preco;
                receitaTotal += receita;
            }
        });

        // Gerar PDF com jsPDF
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        let y = 20;

        // Cabeçalho
        doc.setFillColor(33, 150, 243);
        doc.rect(0, 0, 210, 20, "F");
        doc.setFontSize(16);
        doc.setTextColor(255, 255, 255);
        doc.text("Relatório do Evento", 105, 13, null, null, "center");

        doc.setTextColor(0, 0, 0);
        doc.setFontSize(12);
        y = 30;

        // Informações do evento
        doc.setFont(undefined, "bold");
        doc.text("Informações do Evento:", 14, y); y += 8;
        doc.setFont(undefined, "normal");

        const info = [
            ["Nome", data.nome],
            ["Descrição", data.descricao],
            ["Data", data.data.split("T")[0]],
            ["Hora", data.hora],
            ["Local", data.local],
            ["Categoria", data.categoriaNome],
            ["Capacidade", String(data.capacidade)],
            ["Inscritos", String(vendidos ?? "N/A")]
        ];

        info.forEach(([label, valor]) => {
            doc.text(`${label}:`, 20, y);
            doc.text(valor, 60, y);
            y += 7;
        });

        y += 5;

        // Receita total
        doc.setFont(undefined, "bold");
        doc.text("Receita Total:", 14, y); y += 8;
        doc.setFont(undefined, "normal");
        doc.text(`€ ${receitaTotal.toFixed(2)}`, 20, y); y += 10;

        // Separador
        doc.setDrawColor(200, 200, 200);
        doc.line(20, y, 190, y);
        y += 10;

        // Atividades
        doc.setFont(undefined, "bold");
        doc.setFontSize(13);
        doc.text("Atividades do Evento", 14, y);
        y += 8;
        doc.setFont(undefined, "normal");
        doc.setFontSize(11);

        if (atividades.length === 0) {
            doc.text("Sem atividades registadas.", 20, y); y += 10;
        } else {
            atividades.forEach(atv => {
                if (y > 270) { doc.addPage(); y = 20; }

                doc.setFont(undefined, "bold");
                doc.text(`• ${atv.nome}`, 20, y); y += 6;

                doc.setFont(undefined, "normal");
                doc.text(`Data: ${atv.data}    Hora: ${atv.hora}`, 25, y); y += 6;
                doc.text(`Capacidade: ${atv.quantidademaxima}`, 25, y); y += 6;

                if (atv.descricao) {
                    const splitDescricao = doc.splitTextToSize(`Descrição: ${atv.descricao}`, 160);
                    doc.text(splitDescricao, 25, y);
                    y += splitDescricao.length * 6;
                }

                y += 4;
            });
        }

        doc.save(`Evento_${data.nome.replace(/\s+/g, "_")}.pdf`);
    } catch (err) {
        console.error("Erro ao gerar relatório:", err);
        await Swal.fire({
            icon: 'error',
            title: 'Erro',
            text: 'Não foi possível gerar o relatório.',
            timer: 2000,
            showConfirmButton: false
        });
    }
}


let ingressoSelecionadoId = null;
let eventoSelecionadoId = null;

// Ao clicar no botão "Inscrever-me"
document.body.addEventListener("click", async (e) => {
    if (e.target.classList.contains("inscrever-btn")) {
        eventoSelecionadoId = e.target.dataset.id;

        try {
            const response = await fetch(`/api/ingressos/por-evento/${eventoSelecionadoId}`, {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
                }
            });

            if (!response.ok) throw new Error("Erro ao buscar ingressos.");

            const ingressos = await response.json();
            const select = document.getElementById("selectIngresso");
            select.innerHTML = "";
            if (!ingressos || ingressos.length === 0) {
                await Swal.fire({
                    icon: 'info',
                    title: 'Ingressos Esgotados',
                    text: 'Não há ingressos disponíveis para este evento.',
                    confirmButtonText: 'Ok'
                });
                return;
            }
            ingressos.forEach(ing => {

                const option = document.createElement("option");
                option.value = ing.idingresso;
                option.textContent = `${ing.nomeingresso} - ${ing.preco.toFixed(2)} €`;
                select.appendChild(option);
            });

            $('#modalIngressos').modal('show');

        } catch (err) {
            console.error("Erro ao carregar ingressos:", err);
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao carregar os ingressos.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    }
});

// Ao confirmar escolha de ingresso no modal
document.getElementById("confirmarIngressoBtn").addEventListener("click", async () => {
    ingressoSelecionadoId = document.getElementById("selectIngresso").value;

    if (!ingressoSelecionadoId) {
        await Swal.fire({
            icon: 'warning',
            title: 'Selecione um ingresso',
            text: 'Por favor, selecione um ingresso antes de confirmar.',
            timer: 2000,
            showConfirmButton: false
        });
        return;
    }

    $('#modalIngressos').modal('hide');

    const confirmacao = await Swal.fire({
        title: "Confirmar inscrição?",
        text: "Deseja inscrever-se com o ingresso selecionado?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Sim, Comprar",
        cancelButtonText: "Cancelar"
    });

    if (!confirmacao.isConfirmed) return;

    const token = localStorage.getItem("jwtToken");
    if (!token) {
        await Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'warning',
            title: 'Sessão expirada',
            text: 'Por favor, inicie sessão.',
            timer: 2000,
            showConfirmButton: false
        });
        window.location.href = "/login";
        return;
    }

    try {
        const select = document.getElementById("selectIngresso");
        const selectedOption = select.options[select.selectedIndex];
        const nomeIngresso = selectedOption.textContent.split(" - ")[0];
        const precoTexto = selectedOption.textContent.split(" - ")[1].replace("€", "").trim();
        const preco = parseFloat(precoTexto.replace(",", "."));

        const response = await fetch("/carrinho/adicionar", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                idIngresso: parseInt(ingressoSelecionadoId),
                nomeIngresso: nomeIngresso,
                preco: preco
            })
        });

        if (response.ok) {
            await Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: 'Ingresso adicionado ao carrinho!',
                timer: 2000,
                showConfirmButton: false
            });
        } else {
            const msg = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: msg,
                timer: 2000,
                showConfirmButton: false
            });
        }
    } catch (err) {
        console.error("Erro ao adicionar ao carrinho:", err);
        await Swal.fire({
            icon: 'error',
            title: 'Erro',
            text: 'Erro ao comunicar com o servidor.',
            timer: 2000,
            showConfirmButton: false
        });
    }
});
document.body.addEventListener("click", async (e) => {
    if (e.target.classList.contains("cancelar-btn")) {
        const id = e.target.dataset.id;

        const confirmacao = await Swal.fire({
            title: "Cancelar inscrição?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sim, cancelar",
            cancelButtonText: "Não"
        });

        if (!confirmacao.isConfirmed) return;

        try {
            const response = await fetch(`/api/eventos/${id}/cancelar`, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
                }
            });

            if (response.ok) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Inscrição cancelada!',
                    timer: 2000,
                    showConfirmButton: false
                });
                location.reload();
            } else {
                const msg = await response.text();
                await Swal.fire({
                    icon: 'error',
                    title: 'Erro ao cancelar',
                    text: msg,
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        } catch (err) {
            console.error("Erro ao cancelar inscrição:", err);
            await Swal.fire({
                icon: 'error',
                title: 'Erro de ligação',
                text: 'Erro ao comunicar com o servidor.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    }
});