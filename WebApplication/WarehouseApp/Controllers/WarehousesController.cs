using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.DTOs;
using WebApplication.Repositories;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {

        private readonly IWarehouseRepository _warehouseRepository;

        public WarehousesController(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(DataToAccept dataToAccept)
        {
            
            try
            {
                await _warehouseRepository.ProductWithIdExists(dataToAccept.IdProduct);
                await _warehouseRepository.WarehouseWithIdExists(dataToAccept.IdWarehouse);
                await _warehouseRepository.IdProductAndAmountCombinationInOrderExists(dataToAccept.IdProduct,
                    dataToAccept.Amount);
                var orderId = await _warehouseRepository.ReturnOrderId(dataToAccept.IdProduct, dataToAccept.Amount);
                await _warehouseRepository.CreatedAtOfOrderIsLowerThanCreatedAtOfRequest(orderId, dataToAccept.Date);
                await _warehouseRepository.IdOrderDoesNotAlreadyExist(orderId);
                await _warehouseRepository.UpdateFulfilledAtOfOrder(orderId);
                var newId = _warehouseRepository.InsertProduct_WarehouseRecord(dataToAccept, orderId);

                return Ok(newId.Result);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
            
                
          
        }
    }
}
