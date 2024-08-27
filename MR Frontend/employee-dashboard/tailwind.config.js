/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {

      colors: {
        skyBlue: 'var(--col_blue)',
        lavender: 'var(--col_lavender)',
        white: 'var(--col_white)',
        yel: 'var(--col_regyellow)',
      },

    },
  },
  plugins: [],
};
