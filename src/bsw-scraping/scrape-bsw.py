import scrapy

class BSWCCTVKotaBogorSpider(scrapy.Spider):
    name = "bsw_cctv_kota_bogor"
    allowed_domains = ["bsw.kotabogor.go.id"]
    start_urls = ["https://bsw.kotabogor.go.id/cctv"]

    def parse(self, response):
        # extract URLs from list page
        if "detail" not in response.url:
            for cctv_url in response.xpath('//a[contains(@href, "/detail") and @target="_blank"]/@href').getall():
                yield response.follow(cctv_url)

            return
        
        # extract HLS streaming URL
        title = response.xpath('//h2/span/text()').get()
        hls_url = response.xpath('//source/@src').get()

        yield {
            "title": title,
            "url": hls_url,
        }
