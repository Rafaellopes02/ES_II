document.addEventListener("DOMContentLoaded", async () => {
    await carregarUtilizadores();
    carregarPaises();
});

async function carregarUtilizadores() {
    const res = await fetch("/api/manager", { credentials: "include" });
    const utilizadores = await res.json();
    const tbody = document.querySelector("#tabelaUtilizadores tbody");
    tbody.innerHTML = "";

    utilizadores.forEach(u => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td><input class="form-control" value="${u.nome}" /></td>
            <td><input class="form-control" value="${u.nomeutilizador}" /></td>
            <td>
                <span>********</span>
                <button class="btn btn-warning btn-sm" onclick="redefinirSenha(${u.idutilizador})">Redefinir</button>
            </td>
            <td><input class="form-control" value="${u.email}" /></td>
            <td><input class="form-control" value="${u.telefone}" /></td>
            <td>
                <select class="form-control">
                    <option value="2" ${u.idtipoutilizador == 2 ? "selected" : ""}>UserManager</option>
                    <option value="3" ${u.idtipoutilizador == 3 ? "selected" : ""}>Participante</option>
                </select>
            </td>
            <td>
                <button class="btn btn-success btn-sm" onclick="atualizar(${u.idutilizador}, this)">Salvar</button>
                <button class="btn btn-danger btn-sm" onclick="apagar(${u.idutilizador})">Eliminar</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

async function atualizar(id, btn) {
    const tr = btn.closest("tr");
    const dados = {
        nome: tr.children[0].querySelector("input").value,
        nomeutilizador: tr.children[1].querySelector("input").value,
        email: tr.children[3].querySelector("input").value,
        telefone: tr.children[4].querySelector("input").value,
        idtipoutilizador: parseInt(tr.children[5].querySelector("select").value)
    };

    const res = await fetch(`/api/utilizadores/${id}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dados)
    });

    if (res.ok) {
        Swal.fire("Sucesso!", "Utilizador atualizado com sucesso.", "success");
    } else {
        Swal.fire("Erro", "Ocorreu um erro ao atualizar.", "error");
    }
}

async function apagar(id) {
    const confirmacao = await Swal.fire({
        title: "Tem a certeza?",
        text: "Esta ação não pode ser revertida!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sim, eliminar",
        cancelButtonText: "Cancelar"
    });

    if (!confirmacao.isConfirmed) return;

    const res = await fetch(`/api/utilizadores/${id}`, {
        method: "DELETE",
        credentials: "include"
    });

    if (res.ok) {
        Swal.fire("Eliminado!", "O utilizador foi removido com sucesso.", "success").then(() => {
            carregarUtilizadores();
        });
    } else {
        Swal.fire("Erro", "Erro ao eliminar o utilizador.", "error");
    }
}

async function redefinirSenha(id) {
    const { value: novaSenha } = await Swal.fire({
        title: "Redefinir Senha",
        input: "password",
        inputLabel: "Nova senha",
        inputPlaceholder: "Digite a nova senha",
        showCancelButton: true,
        inputAttributes: {
            maxlength: 100,
            autocapitalize: "off",
            autocorrect: "off"
        }
    });

    if (!novaSenha) return;

    const res = await fetch(`/api/utilizadores/${id}/redefinir-senha`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ novaSenha })
    });

    if (res.ok) {
        Swal.fire("Sucesso!", "Senha redefinida com sucesso.", "success");
    } else {
        Swal.fire("Erro", "Erro ao redefinir a senha.", "error");
    }
}

async function criarUtilizador() {
    const nome = document.getElementById("inputNome").value;
    const email = document.getElementById("inputEmail").value;
    const idade = parseInt(document.getElementById("inputIdade").value);
    const telefone = document.getElementById("inputTelefone").value;
    const nacionalidade = document.getElementById("inputNacionalidade").value;
    const nomeutilizador = document.getElementById("inputUsername").value;
    const senha = document.getElementById("inputSenha").value;
    const idtipoutilizador = parseInt(document.getElementById("inputTipo").value);

    const dados = { nome, email, idade, telefone, nacionalidade, nomeutilizador, senha, idtipoutilizador };

    const res = await fetch("/api/utilizadores", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dados)
    });

    if (res.ok) {
        Swal.fire("Sucesso!", "Utilizador criado com sucesso.", "success").then(() => {
            // Fecha corretamente o modal
            const modal = bootstrap.Modal.getInstance(document.getElementById("modalNovoUtilizador"));
            modal.hide();

            // Atualiza a tabela sem recarregar a página
            carregarUtilizadores(); // ou adicionar só o novo utilizador manualmente
        });

    } else {
        const erro = await res.json();
        const msg = erro?.errors?.Nacionalidade?.[0] || "Erro ao criar utilizador.";
        Swal.fire("Erro", msg, "error");
    }
}

function carregarPaises() {
    const select = document.getElementById("inputNacionalidade");
    if (!select || typeof paises === "undefined") return;

    paises.forEach(pais => {
        const option = document.createElement("option");
        option.value = pais;
        option.textContent = pais;
        select.appendChild(option);
    });
}
