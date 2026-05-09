namespace Tests
{
   public class TestDePrueba
   {
     
         [Fact]
         public void Test1()
         {
            Assert.True(true);

         }

         [Fact]
         public void Otro_test_que_da_true()
         {
            Assert.True(true);

         }

         [Fact]
         public void Test_que_falla_para_testear_CI()
         {
            Assert.True(false);

         }

      
   }
}