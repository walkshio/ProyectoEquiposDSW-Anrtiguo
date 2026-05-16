document.querySelectorAll('.equipo-radio').forEach(radio => {
    radio.addEventListener('change', function () {
        document.querySelectorAll('.equipo-card').forEach(card => {
            card.classList.remove('selected');
        });

        if (this.checked) {
            this.nextElementSibling.classList.add('selected');
        }
    });
});

document.querySelectorAll('.equipo-card').forEach(card => {
    card.addEventListener('click', function () {
        const radio = this.previousElementSibling;
        radio.checked = true;
        radio.dispatchEvent(new Event('change'));
    });
});
