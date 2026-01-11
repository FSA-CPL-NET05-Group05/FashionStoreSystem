# --- Stage 1: Build Source Code ---
# Dùng Node để build code Angular ra HTML/CSS/JS
FROM node:18-alpine as build
WORKDIR /app

# Copy package.json trước để tận dụng cache của Docker
COPY package*.json ./
RUN npm install

# Copy toàn bộ code và chạy lệnh build
COPY . .
RUN npm run build --prod

# --- Stage 2: Serve bằng Nginx ---
# Dùng Nginx siêu nhẹ để chạy web
FROM nginx:alpine

# COPY file đã build từ Stage 1 sang folder của Nginx
# LƯU Ý: Bạn cần kiểm tra folder 'dist' của bạn. 
# Angular 17+ thường là: /dist/fashion-store/browser
# Angular cũ hơn là: /dist/fashion-store
COPY --from=build /app/dist/fashion-store/browser /usr/share/nginx/html

# Copy file cấu hình Nginx (ta sẽ tạo ở dưới)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80