
if (window.location.pathname === "/" || window.location.pathname === "/Dashboard") {
    localStorage.removeItem("jwtToken");
}


document.querySelectorAll('.enviarMensagemBtn').forEach(botao => {
    botao.addEventListener('click', function () {
        const mensagemTexto = document.getElementById('mensagemTexto').value;
        const eventoId = parseInt(document.getElementById('eventoId').value);
        const destinatarioId = parseInt(this.getAttribute('data-destinatario-id'));

        fetch('/Mensagens/EnviarMensagem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                conteudo: mensagemTexto,
                eventoId: eventoId,
                destinatarioId: destinatarioId
            })
        })
        .then(response => {
            if (response.ok) {
                alert('Mensagem enviada com sucesso!');
                location.reload();
            } else {
                alert('Erro ao enviar mensagem.');
            }
        });
    });
});
