let ticketIndex = 0;

function carregarTiposIngressos(selectElement) {
    fetch('/api/TiposIngressosApi') // Usa aqui o link confirmado
        .then(response => {
            if (!response.ok) throw new Error("Erro ao buscar os tipos de ingresso");
            return response.json();
        })
        .then(data => {
            selectElement.innerHTML = '<option value="">Selecionar...</option>';
            data.forEach(tipo => {
                const option = document.createElement('option');
                option.value = tipo.idtipoingresso; // nome correto do JSON
                option.textContent = tipo.nome;
                selectElement.appendChild(option);
            });
        })
        .catch(error => console.error('Erro ao carregar tipos de ingresso:', error));
}

document.getElementById('addTicketBtn').addEventListener('click', function () {
    const container = document.getElementById('ticketsContainer');

    const currentIndex = ticketIndex;

    const ticketHTML = `
        <div class="border p-3 mb-3 rounded bg-light">
            <h6>Ingresso ${currentIndex + 1}</h6>
            <div class="form-group">
                <label for="ticketName${currentIndex}">Nome</label>
                <input type="text" class="form-control" id="ticketName${currentIndex}" name="ticketName[]" required>
            </div>
            <div class="form-group">
                <label for="ticketType${currentIndex}">Tipo</label>
                <select class="form-control ticket-type-select" id="ticketType${currentIndex}" name="ticketType[]" required></select>
            </div>
            <div class="form-group">
                <label for="ticketQuantity${currentIndex}">Quantidade</label>
                <input type="number" class="form-control" id="ticketQuantity${currentIndex}" name="ticketQuantity[]" min="1" required>
            </div>
            <div class="form-group">
                <label for="ticketPrice${currentIndex}">Preço (€)</label>
                <input type="number" step="0.01" class="form-control" id="ticketPrice${currentIndex}" name="ticketPrice[]" min="0" required>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', ticketHTML);

    // Esperar que o elemento realmente esteja no DOM
    requestAnimationFrame(() => {
        const selectElement = document.getElementById(`ticketType${currentIndex}`);
        if (selectElement) {
            carregarTiposIngressos(selectElement);
        } else {
            console.error(`Não foi possível encontrar #ticketType${currentIndex} no DOM`);
        }
    });

    ticketIndex++;
});