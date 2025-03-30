document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwtToken");

    // Se não houver token, redireciona para o login
    if (!token) {
        window.location.href = "/login";
        return;
    }

    try {
        const response = await fetch("/eventos/stats", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const data = await response.json();
            const eventList = document.getElementById("eventList");
            eventList.innerHTML = ''; // Limpa conteúdo anterior

            data.eventos.forEach(evento => {
                const formattedDate = new Date(evento.data).toLocaleDateString('pt-PT');

                // Criação do card do evento
                const card = document.createElement("div");
                card.className = "card mb-3 p-3 event-card";

                card.innerHTML = `
                    <h5 class="card-title">${evento.nome}</h5>
                    <p class="card-text">${evento.descricao}</p>
                    <p class="card-text">
                        <small class="text-muted">${formattedDate} · ${evento.hora} · ${evento.local}</small>
                    </p>
                    <div class="progress">
                        <div class="progress-bar bg-info" style="width: 75%">
                            75% da capacidade
                        </div>
                    </div>
                    <div class="mt-3">
                        <button class="btn btn-outline-primary" onclick="location.href='/events/details/${evento.idevento}'">
                            Detalhes
                        </button>
                    </div>
                `;

                eventList.appendChild(card);
            });
        } else if (response.status === 401) {
            alert("Sessão expirada. Por favor, inicie sessão novamente.");
            localStorage.removeItem("jwtToken");
            window.location.href = "/login";
        } else {
            alert("Erro ao buscar eventos.");
        }
    } catch (error) {
        console.error("Erro ao carregar eventos:", error);
        alert("Erro de comunicação com o servidor.");
    }
});
