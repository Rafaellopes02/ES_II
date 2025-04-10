async function filterEvents(tipo) {
    const token = localStorage.getItem("jwtToken");

    if (!token) {
        window.location.href = "/login";
        return;
    }

    let url = "/eventos/stats";

    if (tipo === "futuros") {
        url = "/api/eventos/futuros";
    } else if (tipo === "passados") {
        url = "/api/eventos/passados";
    }

    try {
        const response = await fetch(url, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (response.ok) {
            const data = await response.json();
            renderizarEventos(data.eventos || data);
        } else {
            alert("Erro ao filtrar eventos.");
        }
    } catch (error) {
        console.error("Erro ao filtrar eventos:", error);
        alert("Erro de comunicação com o servidor.");
    }
}

// 👇 Isto é essencial para tornar a função global
window.filterEvents = filterEvents;
