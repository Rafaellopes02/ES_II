document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwtToken");
    if (!token) {
        window.location.href = "/login";
        return;
    }

    const profileStatus = document.getElementById("profileStatus");
    const profileCard = document.getElementById("profileCard");

    try {
        const response = await fetch("/profile/getuserprofile", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            window.location.href = "/login";
            return;
        }

        const data = await response.json();

        document.getElementById("userNome").textContent = data.nome || "";
        document.getElementById("userEmail").textContent = data.email || "";
        document.getElementById("userIdade").textContent = data.idade || "";
        document.getElementById("userTelefone").textContent = data.telefone || "";
        document.getElementById("userNacionalidade").textContent = data.nacionalidade || "";

        profileCard.style.display = "block";
        profileStatus.style.display = "none"; // ✅ Esconde a barra azul
    } catch {
        window.location.href = "/login";
    }
});
