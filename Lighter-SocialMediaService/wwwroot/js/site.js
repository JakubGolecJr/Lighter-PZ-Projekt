<script>
    document.addEventListener('DOMContentLoaded', () => {
        const navLinks = document.querySelectorAll('.nav-animated');

        navLinks.forEach(link => {
        link.addEventListener('mouseover', () => {
            link.style.color = '#ffdde1';
        });

            link.addEventListener('mouseout', () => {
        link.style.color = '#ffffff';
            });
        });
    });
</script>
